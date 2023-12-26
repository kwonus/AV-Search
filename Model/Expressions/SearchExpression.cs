namespace AVSearch.Model.Expressions
{
    using AVSearch.Interfaces;

    public abstract class SearchExpression
    {
        public bool Quoted          { get; protected set; }
        public string Expression    { get; protected set; }
        public UInt16 ExpressionIdx { get; protected set; }
        public List<SearchFragment> Fragments { get; protected set; }

        public ISettings Settings   { get; protected set; }
        public Dictionary<string, SearchFilter> Filters { get; protected set; }
        public bool Valid           { get; protected set; }

        protected SearchExpression()
        {
            this.Quoted = false;
            this.Expression = string.Empty;
            this.ExpressionIdx = 0;
            this.Fragments = new();
            this.Valid = false;
        }
    }
}
