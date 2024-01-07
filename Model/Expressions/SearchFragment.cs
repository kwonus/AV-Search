using AVSearch.Model.Results;

namespace AVSearch.Model.Expressions
{
    public abstract class SearchFragment
    {
        public UInt64 Hits { get; protected set; }
        public bool Anchored { get; protected set; }
        public string Fragment { get; protected set; }
        public ushort FragmentIdx { get; protected set; }
        public List<SearchMatchAny> AllOf { get; protected set; }
        protected SearchFragment(string text)
        {
            this.Fragment = text;
            this.AllOf = new();
        }
        public UInt16 Compare(QueryResult result, ref AVXLib.Memory.Written writ)
        {
            return 0;
        }
    }

}
