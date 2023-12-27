namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;
    using AVXLib;
    using AVXLib.Memory;
    using Interfaces;
    using System;
    using System.Linq;
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
        private void SearchQuoted(SearchExpression expression, (byte chapter, byte verse) from, (byte chapter, byte verse) to)
        {
            var bk = ObjectTable.AVXObjects.Mem.Book.Slice(this.BookNum).Span[0];
            var book = bk.written;

            // TODO loop through book: TODO as of 2023/12/26
        }
        private void SearchUnquoted(SearchExpression expression, (byte chapter, byte verse) from, (byte chapter, byte verse) to)
        {
            var bk = ObjectTable.AVXObjects.Mem.Book.Slice(this.BookNum).Span[0];
            var book = bk.written;

            // TODO loop through book: TODO as of 2023/12/26
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