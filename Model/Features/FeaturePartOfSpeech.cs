namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeaturePartOfSpeech : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public UInt16 PnPos12 { get; }
        public UInt32 Pos32 { get; }

        public override UInt16 Compare(ref AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeaturePartOfSpeech(string text, bool negate) : base(text, negate)
        {
            this.PnPos12 = 0;
            this.Pos32 = 0;
        }
    }
}