namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeaturePunctuation : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        byte Punctuation { get; }

        public override UInt16 Compare(ref AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeaturePunctuation(string text, bool negate) : base(text, negate)
        {
            this.Punctuation = 0;
        }
    }
}