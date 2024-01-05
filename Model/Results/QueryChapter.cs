namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;

    public class QueryChapter : TypeChapter
    {
        public QueryChapter(byte num)
        {
            this.Matches = new();
            this.TotalHits = 1;
        }
        public bool AddScope(SearchFilter.ChapterRange range)
        {
            return false;
        }
    }
}
