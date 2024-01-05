namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
 
    public class QueryMatch
    {
        public QueryMatch(AVXLib.Memory.BCVW start, ref SearchExpression exp, SearchFragment frag)
        {
            this.Expression = exp;
            this.Fragment = frag;

            this.Highlights = new();

            this.Start = start;
            this.Until = start;
        }
        public AVXLib.Memory.BCVW Start { get; private set; }
        public AVXLib.Memory.BCVW Until { get; private set; }

        public bool Add(ref QueryTag match)
        {
            this.Highlights.Add(match);
            if (match.Coordinates > this.Until)
                this.Until = match.Coordinates;
            return true;
        }

        public List<QueryTag> Highlights { get; private set; }
        public SearchExpression Expression { get; private set; }
        public SearchFragment Fragment { get; private set; }

    }
}
