namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;

    public class QueryChapter : TypeChapter
    {
        public QueryChapter(byte num)
        {
            Matches = new();
        }
        public bool AddScope(SearchFilter.ChapterRange range)
        {
            return false;
        }
    }
}
