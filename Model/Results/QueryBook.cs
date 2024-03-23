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
        public Dictionary<byte, ScopingFilter> Scope;
        public QueryBook(byte num) : base()
        {
            this.BookNum = num;
            this.Chapters = new();
            this.ChapterRange = new();
            this.Scope = new();
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
                    foreach (var range in this.Scope)
                    {
                        (byte chapter, byte verse) from = (range.Key, 0);
                        (byte chapter, byte verse) to   = (range.Key, 0);

                        if ((from.chapter == to.chapter) && this.ChapterRange.Contains(from.chapter))
                            continue; // we have already searched this entire chapter.
// TO DO: TODO
/*
                        foreach (var range in range.Value)
                        {
                            from.verse = range.Verse.from;
                            to.verse   = range.Verse.to;

                            if (expression.Quoted)
                                SearchQuoted(expression, from, to);
                            else
                                SearchUnquoted(expression, from, to);
                        }
*/
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

            while (wi < wend)
            {
                if (prematureChapter && (writ[(int)wi].BCVWc.C > until.chapter))
                    break;
                if (prematureVerse && (writ[(int)wi].BCVWc.C == until.chapter) && (writ[(int)wi].BCVWc.V > until.verse))
                    break;

                byte c = writ[(int)wi].BCVWc.C;

                QueryMatch match = new(writ[wi].BCVWc, ref expression);

                foreach (SearchFragment fragment in fragments.Values)
                {
                    bool found = false;
                    bool all_of_match_success = false;
                    UInt32 all_of_match_cnt = 0;
                    foreach (SearchMatchAny options in fragment.AllOf)
                    {
                        if (wi >= wend)
                        {
                            return false;
                        }
                        foreach (FeatureGeneric feature in options.AnyFeature)
                        {
                            UNANCHORED_RETRY:
                            if (wi >= wend)
                            {
                                return false;
                            }
                            QueryTag tag = new(fragment, options, feature, writ[wi].BCVWc);

                            (byte word, byte lemma) thresholds = expression.Settings.SearchSimilarity;

                            UInt16 score = feature.Compare(writ[wi], ref match, ref tag);
                            found = (score > 0) && (score >= expression.Settings.SearchSimilarity.word);

                            if (found)
                            {
                                all_of_match_cnt++;
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

                                match.Add(ref tag);
                                wi++;
                                break;
                            }
                            if (!fragment.Anchored)
                            {
                                wi++;
                                goto UNANCHORED_RETRY;
                            }
                        }
                        all_of_match_success = (all_of_match_cnt == fragment.AllOf.Count);
                        if (all_of_match_success)
                        {
                            break;
                        }
                        else if (fragment.Anchored)
                        {
                            return false; // anchored needs this w to be a match
                        }
                    }   // end: MatchAll
                }   // end: foreach fragment

                if (match.Highlights.Count == fragments.Count)
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
                    UInt32 cnt = (UInt32)(bk.Matches.Count + 1);
                    bk.Matches[cnt] = match;

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

            QueryMatch? match = null;
            int fragCnt = fragments.Count;
            Span<UInt16> fragFinds = stackalloc UInt16[fragCnt];
            for (int i = 0; i < fragCnt; i++)
                fragFinds[i] = 0;
            for (int wi = (int)w; wi < wend; wi++)
            {
                int f = -1;
                foreach (SearchFragment fragment in fragments.Values)
                {
                    ++f;
                    if (prematureChapter && (writ[(int)wi].BCVWc.C > until.chapter))
                        break;
                    if (prematureVerse && (writ[(int)wi].BCVWc.C == until.chapter) && (writ[(int)wi].BCVWc.V > until.verse))
                        break;

                    foreach (SearchMatchAny options in fragment.AllOf)
                    {
                        foreach (FeatureGeneric feature in options.AnyFeature)
                        {
                            QueryTag tag = new(fragment, options, feature, writ[wi].BCVWc);

                            (byte word, byte lemma) thresholds = expression.Settings.SearchSimilarity;

                            UInt16 score = feature.Compare(writ[wi], ref match, ref tag);
                            bool found = (score > 0) && (score >= expression.Settings.SearchSimilarity.word);

                            if (!found)
                                continue; // ... look for next options.AnyFeature

                            fragFinds[f]++;

                            BCVW coordinates = writ[wi].BCVWc;

                            if (match == null)
                            {
                                match = new(coordinates, ref expression);
                            }
                            if (!hits.ContainsKey(coordinates))
                            {
                                hits[coordinates] = new() { feature.Text };
                                feature.IncrementHits();
                            }
                            else
                            {
                                HashSet<string> features = hits[coordinates];

                                // Avoid double [redundant] counting of feature hits
                                //
                                if (!features.Contains(feature.Text))
                                {
                                    features.Add(feature.Text);
                                    feature.IncrementHits();
                                }
                            }

                            match.Add(ref tag);

                            // This fragment was found, have all fragments been found?
                            for (int i = 0; i < fragCnt; i++)
                                if (fragFinds[i] == 0)
                                    goto NEXT_FRAGMENT;

                            // Otherwise, all fragments have been found
                            byte b = book.bookNum;
                            byte c = writ[(int)w].BCVWc.C;

                            expression.IncrementHits();
                            this.TotalHits++;

                            QueryBook bk = expression.Books[b];
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
                            UInt32 cnt = (UInt32)(bk.Matches.Count + 1);
                            bk.Matches[cnt] = match;

                            return true;
                        }
                    }   // end: MatchAll

                NEXT_FRAGMENT:
                ;
                }   // end: foreach fragment
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
        public Dictionary<byte, QueryChapter> Chapters { get; private set; }

        public void IncrementHits()
        {
            this.TotalHits++;
        }
    }
}