namespace AVSearch.Model.Results
{
    using AVSearch.Model.Types;

    public class QueryChapter : TypeChapter
    {
        public QueryChapter(byte num)
        {
            Matches = new();
        }
    }
}
