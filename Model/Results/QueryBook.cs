namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;
    using Interfaces;

    public class QueryBook : TypeBook
    {
        public QueryBook(byte num)
        {

        }
        public string fetch(byte chapter_num)
        {
            return string.Empty;
        }
        public bool search(ref SearchExpression expression, ref ISettings settings, ref List<uint> scope)
        {
            return expression.Quoted
                ? search_quoted(ref expression, ref settings, ref scope)
                : search_unquoted(ref expression, ref settings, ref scope);
        }
        private bool search_quoted(ref SearchExpression expression, ref ISettings settings, ref List<uint> scope)
        {
            return false;
        }
        private bool search_unquoted(ref SearchExpression expression, ref ISettings settings, ref List<uint> scope)
        {
            return false;
        }
        private Dictionary<byte, QueryChapter> chapters;

    }
}