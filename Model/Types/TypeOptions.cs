namespace AVSearch.Model.Types
{
    using AVSearch.Model.Features;

    public abstract class TypeOptions
    {
        public List<FeatureGeneric> AnyOf { get; protected set; }
        public string Options { get; protected set; }
        public ushort OptionsIdx { get; protected set; }
        public ulong Hits { get; protected set; }
    }
}
