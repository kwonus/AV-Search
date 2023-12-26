namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;
    using Interfaces;
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
            return expression.Quoted
                ? SearchQuoted(expression)
                : SearchUnquoted(expression);
        }
        private bool SearchQuoted(SearchExpression expression)
        {
            return false;
        }
        private bool SearchUnquoted(SearchExpression expression)
        {
            return false;
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