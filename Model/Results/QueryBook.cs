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
        public QueryBook(byte num) : base()
        {
            this.BookNum = num;
            this.Chapters = new();
        }
        public bool Search(SearchExpression expression)
        {
            bool result = (expression.Fragments.Count > 0);

            if (result)
            {
                if (expression.Quoted)
                    SearchQuoted(expression);
                else
                    SearchUnquoted(expression);
            }
            return result;
        }

        private bool SearchQuotedUsingSpan(ref readonly Book book, ref readonly ReadOnlySpan<Chapter> chapters, ref readonly ReadOnlySpan<Written> writ, ref SearchExpression expression, in Dictionary<string, SearchFragment> fragments, in UInt32 w, in BCVW bcvw)
        {
            int wi = (int)w;
            UInt16 span = expression.Settings.SearchSpan;

            UInt16 wcnt = span > 0 ? span : writ[(int)wi].BCVWc.WC;
            if ((wi + wcnt) > book.writCnt)
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
                byte c = writ[(int)wi].BCVWc.C;
                if (!expression.Scope.InScope(book.bookNum, c))
                {
                    wi += chapters[c-1].writCnt;
                    continue;
                }
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
        private bool SearchQuoted(SearchExpression expression)
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
            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            bool found = false;

            for (UInt32 w = 0; w < book.writCnt; w++)
            {
                BCVW bcvw = writ[(int)w].BCVWc;
                byte c = bcvw.C;
                if (!expression.Scope.InScope(book.bookNum, c))
                {
                    w += chapters[c - 1].writCnt;
                    w --; // because it will be incremented again by the for loop
                    continue;
                }
                if (SearchQuotedUsingSpan(ref book, ref chapters, ref writ, ref expression, in normalizedFragments, in w, in bcvw))
                {
                    found = true;
                }
            }
            return found;
        }
        private bool SearchUnquotedUsingSpan(ref readonly Book book, ref readonly ReadOnlySpan<Chapter> chapters, ref readonly ReadOnlySpan<Written> writ, ref SearchExpression expression, in Dictionary<string, SearchFragment> fragments, in UInt32 w, in BCVW bcvw)
        {
            UInt16 span = expression.Settings.SearchSpan;

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
                byte b = book.bookNum;
                byte c = writ[(int)wi].BCVWc.C;
                if (!expression.Scope.InScope(book.bookNum, c))
                {
                    wi += chapters[c - 1].writCnt;
                    wi--; // because it will be incremented again by the for loop
                    continue;
                }
                int f = -1;
                foreach (SearchFragment fragment in fragments.Values)
                {
                    ++f;

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

        private bool SearchUnquoted(SearchExpression expression)
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
            var writ = book.written.Slice(0, (int)book.writCnt).Span;
            UInt16 span = expression.Settings.SearchSpan;
            int finds = 0;
            for (UInt32 w = 0; w < book.writCnt; /**/)
            {
                BCVW bcvw = writ[(int)w].BCVWc;
                byte c = bcvw.C;
                if (!expression.Scope.InScope(book.bookNum, c))
                {
                    w += chapters[c-1].writCnt;
                    continue;
                }
                bool found = SearchUnquotedUsingSpan(ref book, ref chapters, ref writ, ref expression, in normalizedFragments, in w, in bcvw);
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