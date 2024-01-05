namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureDecoration : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public byte Decoration { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeatureDecoration(string text, bool negate) : base(text, negate)
        {
            this.Decoration = 0;
        }
    }
}