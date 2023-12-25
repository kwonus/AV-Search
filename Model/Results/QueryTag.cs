namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;

    public class QueryTag
    {
        public uint Coordinates { get; private set; }
        public SearchMatchAny Options { get; private set; }
        public FeatureGeneric Feature { get; private set; }

        public QueryTag(SearchMatchAny options, FeatureGeneric feature, uint coordinates = 0)
        {
            Options = options;
            Feature = feature;
            Coordinates = Coordinates;
        }

        byte get_book()
        {
            return (byte)(Coordinates & 0xFF00 >> 24);
        }
        byte get_chapter()
        {
            return (byte)(0xFF & Coordinates >> 16);
        }
        byte get_verse()
        {
            return (byte)(0xFF & Coordinates >> 8);
        }
        byte word()
        {
            return (byte)(0xFF & Coordinates);
        }
        static uint CreateCoordinate(byte b, byte c, byte v, byte w)
        {
            return (uint)b << 24 | (uint)c << 16 | (uint)v << 8 | w;
        }
    }
}
