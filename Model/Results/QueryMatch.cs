namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
 
    public class QueryMatch
    {
        public QueryMatch()
        {
            highlights = new();
        }

        public uint start { get; private set; }
        public uint until { get; private set; }

        public bool Add(ref QueryTag match)
        {
            return false;
        }

        public List<QueryTag> highlights { get; private set; }
        public SearchExpression expression { get; private set; }
        public SearchFragment fragment { get; private set; }

        public QueryResult? find()
        {
            return null;
        }
    }
}
