namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib;
    using System;
    using AVSearch.Model.Results;
    using AVXLib.Memory;
    using PhonemeEmbeddings;

    public class QueryBook : TypeBook
    {
        public HashSet<byte> ChapterRange;
        public Dictionary<byte, List<SearchFilter.VerseRange>> ChapterVerseRange;
        public QueryBook(byte num)
        {
            this.Chapters = new();
            this.ChapterRange = new();
            this.ChapterVerseRange = new();
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

            for (/**/; w + fragCnt - 1 < book.writCnt; /* increment is at end of loop */)
            {
                if (prematureChapter && (writ[(int)w].BCVWc.C > until.chapter))
                    break;
                if (prematureVerse && (writ[(int)w].BCVWc.C == until.chapter) && (writ[(int)w].BCVWc.V > until.verse))
                    break;

                UInt16 localHits = 0;

                Dictionary<BCVW, HashSet<string>> hits = new();
                Dictionary<string, List<QueryMatch>> matches = new();
                int wend = (int)w + fragCnt;
                byte c = writ[(int)w].BCVWc.C;
                for (int wi = (int)w; wi < wend; wi++)
                {
                    matches.Clear();
                    foreach (SearchFragment fragment in expression.Fragments)
                    {
                        bool success = false;
                        UInt32 matched = 0;
                        foreach (SearchMatchAny options in fragment.AllOf)
                        {
                            QueryMatch match = new(writ[wi].BCVWc, ref expression, fragment);

                            foreach (FeatureGeneric feature in options.AnyOf)
                            {
                                QueryTag tag = new(options, feature, writ[wi].BCVWc);

                                if (feature.Compare(writ[wi], ref match, ref tag) >= expression.Settings.SearchSimilarity)
                                {
                                    matched++;
                                    // Avoid double [redundant] counting of feature hits
                                    //
                                    BCVW coordinates = writ[wi].BCVWc;
                                    if (!hits.ContainsKey(coordinates))
                                    {
                                        hits[coordinates] = new() { feature.Text };
                                        feature.IncrementHits();
                                    }
                                    else
                                    { 
                                        HashSet<string> features = hits[coordinates];

                                        if (!features.Contains(feature.Text))
                                        {
                                            features.Add(feature.Text);
                                            feature.IncrementHits();
                                        }
                                    }
                                    // END double/redundant counting logic

                                    if (!matches.ContainsKey(fragment.Fragment))
                                    {
                                        matches[fragment.Fragment] = new();
                                    }
                                    match.Add(ref tag);
                                    matches[fragment.Fragment].Add(match);
                                    break;
                                }
                            }
                            if (matched == fragment.AllOf.Count)
                            {
                                success = true;
                                break;
                            }
                            else if (!fragment.Anchored)
                            {
                                success = false;

                                if (wi+1 < wend)
                                {
                                    wi++;
                                    continue;
                                }
                                else break;
                            }
                            else
                            {
                                success = false;
                                break;
                            }
                        }
                        if (!success)
                        {
                            break;
                        }
                        if (matches.Count == expression.Fragments.Count)
                        {
                            hit = true;
                            expression.IncrementHits();
                            this.TotalHits++;

                            QueryBook bk = expression.Books[book.bookNum];
                            bk.IncrementHits();

                            QueryChapter chapter;
                            if (bk.Chapters.ContainsKey(c))
                            {
                                chapter = this.Chapters[c];
                                chapter.IncrementHits();
                            }
                            else
                            {
                                chapter = new(c);
                                this.Chapters[c] = chapter;
                            }
                            foreach (string frag in matches.Keys)
                            {
                                List<QueryMatch> collection = matches[frag];
                                foreach (QueryMatch match in collection)
                                {
                                    chapter.Matches.Add(match);
                                }
                            }
                            break;
                        }
                    }
                }
                if (localHits == 0)
                {
                    hits.Clear();
                    w += (uint)wend;
                }
                else
                {
                    w++;
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

                Dictionary<string, List<QueryMatch>> matches = new();
                int wend = (int)w + fragCnt;
                byte c = writ[(int)w].BCVWc.C;
                for (int wi = (int)w; wi < wend; wi++)
                {
                    matches.Clear();
                    foreach (string key in normalizedFragments.Keys)
                    {
                        SearchFragment fragment = normalizedFragments[key];

                        foreach (SearchMatchAny options in fragment.AllOf)
                        {
                            QueryMatch match = new(writ[wi].BCVWc, ref expression, fragment);

                            foreach (FeatureGeneric feature in options.AnyOf)
                            {
                                QueryTag tag = new(options, feature, writ[wi].BCVWc);

                                if (feature.Compare(writ[wi], ref match, ref tag) >= expression.Settings.SearchSimilarity)
                                {
                                    feature.IncrementHits();

                                    if (!matches.ContainsKey(fragment.Fragment))
                                    {
                                        matches[fragment.Fragment] = new();
                                    }
                                    match.Add(ref tag);
                                    matches[fragment.Fragment].Add(match);
                                }
                            }
                        }
                    }
                    if (matches.Count == normalizedFragments.Count)
                    {
                        hit = true;

                        expression.IncrementHits();
                        this.TotalHits++;

                        QueryBook bk = expression.Books[book.bookNum];
                        bk.IncrementHits();

                        QueryChapter chapter;
                        if (bk.Chapters.ContainsKey(c))
                        {
                            chapter = this.Chapters[c];
                            chapter.IncrementHits();
                        }
                        else
                        {                            
                            chapter = new(c);
                            this.Chapters[c] = chapter;
                        }
                        foreach (string frag in matches.Keys)
                        {
                            List<QueryMatch> collection = matches[frag];
                            foreach (QueryMatch match in collection)
                            {
                                chapter.Matches.Add(match);
                            }
                        }
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
        public Dictionary<byte, QueryChapter> Chapters { get; private set; }

        public void IncrementHits()
        {
            this.TotalHits++;
        }
    }
}