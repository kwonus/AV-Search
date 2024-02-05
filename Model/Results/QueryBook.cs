namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib;
    using System;
    using AVXLib.Memory;

    public class QueryBook : TypeBook
    {
        public HashSet<byte> ChapterRange;
        public Dictionary<byte, List<SearchFilter.VerseRange>> ChapterVerseRange;
        public QueryBook(byte num)
        {
            this.BookNum = num;
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
        private bool SearchQuotedUsingSpan(ref readonly Book book, ref readonly ReadOnlySpan<Chapter> chapters, ref readonly ReadOnlySpan<Written> writ, ref SearchExpression expression, in Dictionary<string, SearchFragment> fragments, in (byte chapter, byte verse) until, in UInt32 w, in BCVW bcvw)
        {
            int wi = (int)w;
            UInt16 span = expression.Settings.SearchSpan;

            bool prematureVerse = (until.verse < chapters[until.chapter - 1].verseCnt);
            bool prematureChapter = (until.chapter < book.chapterCnt);

            UInt16 wcnt = span > 0 ? span : writ[(int)wi].BCVWc.WC;
            if ( (wi + wcnt) > book.writCnt)
            {
                var cnt = book.writCnt - wi;
                if (cnt > UInt16.MaxValue)
                    return false;
                wcnt = (UInt16)cnt;
            }
            int wend = (int)(wi + wcnt);

            Dictionary<BCVW, HashSet<string>> hits = new();
            Dictionary<string, List<QueryMatch>> matches = new();

            while (wi < wend)
            {
                if (prematureChapter && (writ[(int)wi].BCVWc.C > until.chapter))
                    break;
                if (prematureVerse && (writ[(int)wi].BCVWc.C == until.chapter) && (writ[(int)wi].BCVWc.V > until.verse))
                    break;

                byte c = writ[(int)wi].BCVWc.C;

                foreach (SearchFragment fragment in fragments.Values)
                {
                    bool found = false;
                    bool success = false;
                    UInt32 matched = 0;
                    foreach (SearchMatchAny options in fragment.AllOf)
                    {
                        QueryMatch match = new(writ[wi].BCVWc, ref expression, fragment);

                        foreach (FeatureGeneric feature in options.AnyFeature)
                        {
                            if (wi >= wend)
                            {
                                return false;
                            }
                            QueryTag tag = new(options, feature, writ[wi].BCVWc);

                            (byte word, byte lemma) thresholds = expression.Settings.SearchSimilarity;

                            UInt16 score = feature.Compare(writ[wi], ref match, ref tag);
                            found = (score > 0) && (score >= expression.Settings.SearchSimilarity.word);

                            if (found)
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
                                wi++;
                                break;
                            }
                            else
                            {
                                if (fragment.Anchored)
                                {
                                    return false; // anchored needs this w to be a match
                                }
                                wi++;
                            }
                        }
                        success = (matched == fragment.AllOf.Count);
                        if (success)
                            break;
                    }   // end: MatchAll
                    if (!success)
                    {
                        return false;
                    }
                }   // end: foreach fragment

                if (matches.Count == fragments.Count)
                {
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
                    return true;
                }
            }
            return false;
        }
        private bool SearchQuoted(SearchExpression expression, (byte chapter, byte verse) from, (byte chapter, byte verse) to)
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
            if (start.verse < 1 || start.verse > chapters[start.chapter - 1].verseCnt)
                return false;
            if (until.verse < 1)
                return false;
            if (until.verse > chapters[until.chapter - 1].verseCnt)
                until.verse = chapters[until.chapter - 1].verseCnt; // we allow 0xFF to represent the last verse of the chapter here

            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            bool found = false;
            UInt32 bcv = (UInt32)((book.bookNum << 24) | (start.chapter << 16) | (start.verse << 8));
            for (UInt32 w = 0; writ[(int)w].BCVWc.C < start.chapter || writ[(int)w].BCVWc.V < start.verse; w++)
                ;
            for (UInt32 w = 0; w < book.writCnt; w++)
            {
                BCVW bcvw = writ[(int)w].BCVWc;
                if (SearchQuotedUsingSpan(ref book, ref chapters, ref writ, ref expression, in normalizedFragments, in until, in w, in bcvw))
                {
                    found = true;
                }
            }
            return found;
        }
        private bool SearchUnquotedUsingSpan(ref readonly Book book, ref readonly ReadOnlySpan<Chapter> chapters, ref readonly ReadOnlySpan<Written> writ, ref SearchExpression expression, in Dictionary<string, SearchFragment> fragments, in (byte chapter, byte verse) until, in UInt32 w, in BCVW bcvw)
        {
            UInt16 span = expression.Settings.SearchSpan;

            bool prematureVerse = (until.verse < chapters[until.chapter - 1].verseCnt);
            bool prematureChapter = (until.chapter < book.chapterCnt);

            UInt16 wcnt = span > 0 ? span : writ[(int)w].BCVWc.WC;
            if ((w + wcnt) > book.writCnt)
            {
                var cnt = book.writCnt - w;
                if (cnt > UInt16.MaxValue)
                    return false;
                wcnt = (UInt16)cnt;
            }
            int wend = (int)(w + wcnt);

            Dictionary<BCVW, HashSet<string>> hits = new();
            Dictionary<string, List<QueryMatch>> matches = new();

            foreach (SearchFragment fragment in fragments.Values)
            {
                for (int wi = (int)w; wi < wend; wi++)
                {
                    if (prematureChapter && (writ[(int)wi].BCVWc.C > until.chapter))
                        break;
                    if (prematureVerse && (writ[(int)wi].BCVWc.C == until.chapter) && (writ[(int)wi].BCVWc.V > until.verse))
                        break;

                    foreach (SearchMatchAny options in fragment.AllOf)
                    {
                        QueryMatch match = new(writ[wi].BCVWc, ref expression, fragment);

                        foreach (FeatureGeneric feature in options.AnyFeature)
                        {
                            QueryTag tag = new(options, feature, writ[wi].BCVWc);

                            (byte word, byte lemma) thresholds = expression.Settings.SearchSimilarity;

                            UInt16 score = feature.Compare(writ[wi], ref match, ref tag);
                            bool found = (score > 0) && (score >= expression.Settings.SearchSimilarity.word);

                            if (found)
                            {
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
                    }   // end: MatchAll
                }   // end: foreach fragment
                if (matches.Count == fragments.Count)
                {
                    byte c = writ[(int)w].BCVWc.C;

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
                    return true;
                }
            }
            return false;
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
            if (start.verse < 1 || start.verse > chapters[start.chapter - 1].verseCnt)
                return false;
            if (until.verse < 1)
                return false;
            if (until.verse > chapters[until.chapter - 1].verseCnt)
                until.verse = chapters[until.chapter - 1].verseCnt; // we allow 0xFF to represent the last verse of the chapter here

            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            UInt32 bcv = (UInt32)((book.bookNum << 24) | (start.chapter << 16) | (start.verse << 8));
            for (UInt32 w = 0; writ[(int)w].BCVWc.C < start.chapter || writ[(int)w].BCVWc.V < start.verse; w++)
                ;

            UInt16 span = expression.Settings.SearchSpan;
            int finds = 0;
            for (UInt32 w = 0; w < book.writCnt; /**/)
            {
                BCVW bcvw = writ[(int)w].BCVWc;
                bool found = SearchUnquotedUsingSpan(ref book, ref chapters, ref writ, ref expression, in normalizedFragments, in until, in w, in bcvw);
                if (found)
                {
                    finds++;
                }
                if (span == 0)
                {
                    w += bcvw.WC;
                }
                else if (found)
                {
                    w += span;
                }
                else
                {
                    w++;
                }
            }
            return (finds > 0);
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