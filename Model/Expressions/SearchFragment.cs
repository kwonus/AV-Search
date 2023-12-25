namespace AVSearch.Model.Expressions
{
    using AVSearch.Model.Types;
    public abstract class SearchFragment
    {
        public UInt64 Hits { get; protected set; }
        public bool Anchored { get; protected set; }
        public string Fragment { get; protected set; }
        public ushort FragmentIdx { get; protected set; }
        public List<TypeOptions> AllOf { get; protected set; }
        protected SearchFragment()
        {
            this.AllOf = new();
        }
    }

}
