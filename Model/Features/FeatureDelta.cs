namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureDelta : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public bool hasDelta { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeatureDelta(string text, bool negate) : base(text, negate)
        {
            this.hasDelta = false;
        }
    }
}