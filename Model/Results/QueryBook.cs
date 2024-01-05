namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib;
    using AVXLib.Memory;
    using Interfaces;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml.Xsl;
    using static AVSearch.Model.Expressions.SearchFilter;

    public class QueryBook : TypeBook
    {
        public HashSet<byte> ChapterRange;
        public Dictionary<byte, List<SearchFilter.VerseRange>> ChapterVerseRange;
        public QueryBook(byte num)
        {
            this.ChapterRange = new();
        }
        public bool Search(SearchExpression expression)
        {
            bool result = (expression.Fragments.Count > 0);

            if (result)
            {
                var bk = ObjectTable.AVXObjects.Mem.Book.Slice(this.BookNum).Span[0];

                var book = bk.written;

                if (expression.Scope.Count == 0)
                {
                    (byte chapter, byte verse) from = (1, 1);
                    (byte chapter, byte verse) to = (bk.chapterCnt, 0xFF);

                    if (expression.Quoted)
                        SearchQuoted(expression, from, to);
                    else
                        SearchUnquoted(expression, from, to);
                }
                else
                {
                    foreach (byte c in this.ChapterRange)
                    {
                        (byte chapter, byte verse) from = (c, 1);
                        (byte chapter, byte verse) to = (c, 0xFF);

                        if (expression.Quoted)
                            SearchQuoted(expression, from, to);
                        else
                            SearchUnquoted(expression, from, to);
                    }
                    foreach (var rangesByChapter in this.ChapterVerseRange)
                    {
                        (byte chapter, byte verse) from = (rangesByChapter.Key, 0);
                        (byte chapter, byte verse) to   = (rangesByChapter.Key, 0);

                        foreach (var range in rangesByChapter.Value)
                        {
                            from.verse = range.Verse.from;
                            to.verse   = range.Verse.to;

                            if (expression.Quoted)
                                SearchQuoted(expression, from, to);
                            else
                                SearchUnquoted(expression, from, to);
                        }
                    }
                }
            }
            return result;

        }
        private bool SearchQuoted(SearchExpression expression, (byte chapter, byte verse) from, (byte chapter, byte verse) to)
        {
            var book = ObjectTable.AVXObjects.Mem.Book.Slice(this.BookNum).Span[0];
            var chapters = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx, book.chapterCnt).Span;
            var start = from;
            var until = to;
            if (start.chapter < 1 || start.chapter > book.chapterCnt)
                return false;
            if (start.verse < 1 || start.verse > chapters[book.chapterIdx + start.chapter - 1].verseCnt)
                return false;
            if (until.verse < 1)
                return false;
            if (until.verse > chapters[book.chapterIdx + start.chapter - 1].verseCnt)
                until.verse = chapters[book.chapterIdx + start.chapter - 1].verseCnt; // we allow 0xFF to represent the last verse of the chapter here

            bool prematureVerse = (until.verse < chapters[book.chapterIdx + start.chapter - 1].verseCnt);
            bool prematureChapter = (until.chapter < book.chapterCnt);

            var fragCnt = expression.Fragments.Count;

            if (fragCnt < 1)
                return false;

            bool hit = false;

            UInt32 w = 0;

            var bcv = (UInt32)((book.bookNum << 24) | (start.chapter << 16) | (start.verse << 8));

            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            for (w = 0; writ[(int)w].BCVWc.C < start.chapter || writ[(int)w].BCVWc.V < start.verse; w++)
                ;

            if (expression.Settings.SearchSpan > 0)
            {
                UInt32 len = expression.Settings.SearchSpan;
                UInt32 wlen = 0;
                for (/**/; w < book.writCnt; w++)
                {
                    int wi = (int)w;

                    if (prematureChapter && (writ[wi].BCVWc.C > until.chapter))
                        break;
                    if (prematureVerse && (writ[wi].BCVWc.C == until.chapter) && (writ[wi].BCVWc.V > until.verse))
                        break;

                    if (wlen == 0)
                    {
                        wlen = w + len;
                        if (wlen > book.writCnt)
                            wlen = book.writCnt;
                    }
                    foreach (SearchFragment fragment in expression.Fragments)
                    {
                        var all_of_remaining = fragment.AllOf.Count;

                        foreach (SearchMatchAny options in fragment.AllOf)
                        {
                            --all_of_remaining;

                            QueryMatch match = new(writ[wi].BCVWc);

                            foreach (FeatureGeneric feature in options.AnyOf)
                            {
                                QueryTag tag = new(options, feature, writ[wi].BCVWc);
                                if (wi > wlen)
                                    break;
                                match.until = writ[wi].BCVWc;

                                if (feature.Compare(writ[wi], ref match, ref tag) >= expression.Settings.SearchSimilarity && all_of_remaining == 0)
                                {
                                    /*
                                    for (auto t : match->highlights)
                                    {
                                        byte c = 0;
                                        if (match->start == 0)
                                        {
                                            match->start = t->coordinates;
                                            c = t->get_chapter();
                                        }
                                        else if (match->start > t->coordinates)
                                            match->start = t->coordinates;

                                        else if (match->until < t->coordinates)
                                            match->until = t->coordinates;

                                        this->total_hits++;

                                        TChapter* chapter = this->chapters[c];
                                        if (chapter == nullptr)
                                        {
                                            chapter = new TChapter(c);
                                            this->chapters[c] = chapter;
                                        }
                                        chapter->total_hits++;
                                        this->total_hits++;
                                    }
                                    */
                                }
                                else
                                {
                                    if (fragment.Anchored)
                                    {
                                        //delete match.Dispose;
                                        //delete tag;
                                        goto NOT_FOUND_1;
                                    }
                                    if (wi == wlen - 1) // end-of-verse with verse-span granularity
                                    {
                                        //delete match;
                                        //delete tag;
                                        goto NOT_FOUND_1;
                                    }
                                }
                                wi++;
                            }
                        }
                    }
                NOT_FOUND_1:
                    continue;
                }
            }
            else // almost the same as the previous if condition, but dynamic verse-span instead of fixed-length-span
            {
                UInt32 len = expression.Settings.SearchSpan;
                UInt32 wlen = 0;
                for (/**/; w < book.writCnt; w += writ[(int)w].BCVWc.WC)
                {
                    int wi = (int)w;

                    if (prematureChapter && (writ[wi].BCVWc.C > until.chapter))
                        break;
                    if (prematureVerse && (writ[wi].BCVWc.C == until.chapter) && (writ[wi].BCVWc.V > until.verse))
                        break;

                    foreach (SearchFragment fragment in expression.Fragments)
                    {
                        var all_of_remaining = fragment.AllOf.Count;

                        foreach (SearchMatchAny options in fragment.AllOf)
                        {
                            --all_of_remaining;

                            QueryMatch match = new(writ[wi].BCVWc);

                            foreach (FeatureGeneric feature in options.AnyOf)
                            {
                                QueryTag tag = new(options, feature, writ[wi].BCVWc);
                                if (wi > wlen)
                                    break;
                                match.until = writ[wi].BCVWc;

                                if (feature.Compare(writ[wi], ref match, ref tag) >= expression.Settings.SearchSimilarity && all_of_remaining == 0)
                                {
                                    /*
                                    for (auto t : match->highlights)
                                    {
                                        byte c = 0;
                                        if (match->start == 0)
                                        {
                                            match->start = t->coordinates;
                                            c = t->get_chapter();
                                        }
                                        else if (match->start > t->coordinates)
                                            match->start = t->coordinates;

                                        else if (match->until < t->coordinates)
                                            match->until = t->coordinates;

                                        this->total_hits++;

                                        TChapter* chapter = this->chapters[c];
                                        if (chapter == nullptr)
                                        {
                                            chapter = new TChapter(c);
                                            this->chapters[c] = chapter;
                                        }
                                        chapter->total_hits++;
                                        this->total_hits++;
                                    }
                                    */
                                }
                                else
                                {
                                    if (fragment.Anchored)
                                    {
                                        //delete match.Dispose;
                                        //delete tag;
                                        goto NOT_FOUND_2;
                                    }
                                    if (wi == wlen - 1) // end-of-verse with verse-span granularity
                                    {
                                        //delete match;
                                        //delete tag;
                                        goto NOT_FOUND_2;
                                    }
                                }
                                wi++;
                            }
                        }
                    }
                NOT_FOUND_2:
                    continue;
                }
            }
            return hit;
        }
        private bool SearchUnquoted(SearchExpression expression, (byte chapter, byte verse) from, (byte chapter, byte verse) to)
        {
            Dictionary<string, SearchFragment> normalizedFragments = new();
            foreach (SearchFragment frag in expression.Fragments)
            {
                if (!normalizedFragments.ContainsKey(frag.Fragment))
                {
                    normalizedFragments[frag.Fragment] = frag;
                }
            }
            var book = ObjectTable.AVXObjects.Mem.Book.Slice(this.BookNum).Span[0];
            var chapters = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx, book.chapterCnt).Span;
            var start = from;
            var until = to;
            if (start.chapter < 1 || start.chapter > book.chapterCnt)
                return false;
            if (start.verse < 1 || start.verse > chapters[book.chapterIdx + start.chapter - 1].verseCnt)
                return false;
            if (until.verse < 1)
                return false;
            if (until.verse > chapters[book.chapterIdx + start.chapter - 1].verseCnt)
                until.verse = chapters[book.chapterIdx + start.chapter - 1].verseCnt; // we allow 0xFF to represent the last verse of the chapter here

            bool prematureVerse = (until.verse < chapters[book.chapterIdx + start.chapter - 1].verseCnt);
            bool prematureChapter = (until.chapter < book.chapterCnt);

            var fragCnt = expression.Fragments.Count;

            if (fragCnt < 1)
                return false;

            bool hit = false;

            UInt32 w = 0;

            var bcv = (UInt32)((book.bookNum << 24) | (start.chapter << 16) | (start.verse << 8));

            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            for (w = 0; writ[(int)w].BCVWc.C < start.chapter || writ[(int)w].BCVWc.V < start.verse; w++)
                ;

            for (/**/; w + fragCnt - 1 < book.writCnt; w++)
            {
                if (prematureChapter && (writ[(int)w].BCVWc.C > until.chapter))
                    break;
                if (prematureVerse && (writ[(int)w].BCVWc.C == until.chapter) && (writ[(int)w].BCVWc.V > until.verse))
                    break;

                int wend = (int)w + fragCnt;
                for (int wi = (int)w; wi < wend; wi++)
                {
                    Dictionary<string, List<QueryTag>> matches = new();
                    foreach (SearchFragment fragment in normalizedFragments.Values)
                    {
                        foreach (SearchMatchAny options in fragment.AllOf)
                        {
                            QueryMatch match = new(writ[wi].BCVWc);

                            foreach (FeatureGeneric feature in options.AnyOf)
                            {
                                QueryTag tag = new(options, feature, writ[wi].BCVWc);

                                match.until = writ[wi].BCVWc;

                                if (feature.Compare(writ[wi], ref match, ref tag) >= expression.Settings.SearchSimilarity)
                                {
                                    if (!matches.ContainsKey(fragment.Fragment))
                                    {
                                        matches[fragment.Fragment] = new();
                                    }
                                    matches[fragment.Fragment].Add(tag);
                                }
                            }
                        }
                    }
                    if (matches.Count == normalizedFragments.Count)
                    {
                        // TO DO: Add to QueryMatch list for the root chapter
                    }
                }
            }
            return hit;
        }
        public bool AddRestrictiveRange(SearchFilter.RangeFilter range)
        {
            if (range.GetType() == typeof(SearchFilter.ChapterRange))
            {
                var chapterRange = (SearchFilter.ChapterRange)range;
                for (byte c = chapterRange.Chapter.from; c <= chapterRange.Chapter.to; c++)
                {
                    if (!this.ChapterRange.Contains(c))
                    {
                        this.ChapterRange.Add(c);

                        if (this.ChapterVerseRange.ContainsKey(c))
                            this.ChapterVerseRange.Remove(c);   // Whole chapter subsumes this range

                        return true;
                    }
                }
            }
            else if (range.GetType() == typeof(SearchFilter.VerseRange))
            {
                var verseRange = (SearchFilter.VerseRange)range;
                if (this.ChapterRange.Contains(verseRange.Chapter))    // if whole chapter is already included, disregard this filter
                {
                    if (this.ChapterVerseRange.ContainsKey(verseRange.Chapter))
                        this.ChapterVerseRange[verseRange.Chapter].Add(verseRange);
                    else
                        this.ChapterVerseRange[verseRange.Chapter] = [verseRange];

                    return true;
                }
            }
            return false;
        }
        private Dictionary<byte, QueryChapter> chapters;

    }
}