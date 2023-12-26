namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureLemma : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public HashSet<UInt16> Lemmata { get; protected set; }
        public HashSet<string> Phonetics { get; protected set; }

        public override UInt16 Compare(ref AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeatureLemma(string text, bool negate) : base(text, negate)
        {
            this.Lemmata = new();
            this.Phonetics = new();
        }
    }
}