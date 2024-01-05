namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;

    public class QueryTag
    {
        public AVXLib.Memory.BCVW Coordinates { get; private set; }
        public SearchMatchAny Options { get; private set; }
        public FeatureGeneric Feature { get; private set; }

        public QueryTag(SearchMatchAny options, FeatureGeneric feature, AVXLib.Memory.BCVW coordinates)
        {
            Options = options;
            Feature = feature;
            Coordinates = coordinates;
        }

        byte get_book()
        {
            return this.Coordinates.B;
        }
        byte get_chapter()
        {
            return this.Coordinates.C;
        }
        byte get_verse()
        {
            return this.Coordinates.V;
        }
        byte word()
        {
            return this.Coordinates.WC;
        }
    }
}
