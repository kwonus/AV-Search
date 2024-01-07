namespace AVSearch.Model.Expressions
{
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib.Framework;

    public abstract class SearchMatchAny
    {
        public List<FeatureGeneric> AnyFeature { get; protected set; }
        public string Options                  { get; protected set; }
        public ushort OptionsIdx               { get; protected set; }
        public ulong  Hits                     { get; protected set; }

        protected SearchMatchAny(string options)
        {
            this.AnyFeature = new();
            this.Options = options.Trim();
            this.OptionsIdx = 0;
            this.Hits = 0;
        }
    }
}