namespace AVSearch.Model.Expressions
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;

    public abstract class SearchExpression
    {
        public bool Quoted          { get; protected set; }
        public string Expression    { get; protected set; }
        public UInt16 ExpressionIdx { get; protected set; }
        public List<SearchFragment> Fragments { get; protected set; }

        public ISettings Settings   { get; protected set; }
        public SearchScope Scope    { get; protected set; }
        public bool EmptySelection  { get => string.IsNullOrWhiteSpace(this.Expression) && (this.Scope.Count == 0); }
        public bool IsValid         { get; protected set; }
        public Dictionary<byte, QueryBook> Books { get; protected set; }
        public QueryResult Query    { get; protected set; }
        public UInt64 Hits          { get; private set; }

        protected SearchExpression(ISettings settings, QueryResult query)
        {
            this.Quoted = false;
            this.Expression = string.Empty;
            this.ExpressionIdx = 0;
            this.Fragments = new();
            this.Books = new();
            this.Scope = new();
            this.Settings = settings;
            this.Query = query;
            this.IsValid = false;
        }

        public void AddScope(SearchScope scope)
        {
            this.Scope = scope;
        }
        public void IncrementHits()
        {
            this.Hits++;
        }
    }
}
