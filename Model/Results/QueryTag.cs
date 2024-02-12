namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;

    public class QueryTag
    {
        public AVXLib.Memory.BCVW Coordinates { get; private set; }
        public SearchMatchAny Options { get; private set; }
        public FeatureGeneric Feature { get; private set; }
        public SearchFragment Fragment { get; private set; }

        public QueryTag(SearchFragment fragment, SearchMatchAny options, FeatureGeneric feature, AVXLib.Memory.BCVW coordinates)
        {
            this.Fragment = fragment;
            this.Options = options;
            this.Feature = feature;
            this.Coordinates = coordinates;
        }
    }
}
