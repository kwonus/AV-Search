namespace AVSearch.Model.Expressions
{
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib.Framework;

    public abstract class SearchMatchAny
    {
        public List<FeatureGeneric> AnyOf { get; protected set; }
        public string Options             { get; protected set; }
        public ushort OptionsIdx          { get; protected set; }
        public ulong  Hits                { get; protected set; }

        protected SearchMatchAny()
        {
            this.AnyOf = new();
            this.Options = string.Empty;
            this.OptionsIdx = 0;
            this.Hits = 0;
        }
    }
}