namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
 
    public class QueryMatch
    {
        public QueryMatch(AVXLib.Memory.BCVW from, AVXLib.Memory.BCVW to)
        {
            highlights = new();

            this.start = from;
            this.until = to;
        }
        public QueryMatch(AVXLib.Memory.BCVW from)
        {
            highlights = new();

            this.start = from;
            this.until = from;
        }

        public AVXLib.Memory.BCVW start { get; set; }
        public AVXLib.Memory.BCVW until { get; set; }

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
