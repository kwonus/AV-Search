namespace AVSearch.Model.Expressions
{
    public abstract class SearchExpression
    {
        public bool Quoted          { get; protected set; }
        public string Expression    { get; protected set; }
        public UInt16 ExpressionIdx { get; protected set; }
        public List<SearchFragment> Fragments { get; protected set; }
    }
}
