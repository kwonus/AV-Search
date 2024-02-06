namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;

    public abstract class FeaturePartOfSpeech : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public (UInt16 value, UInt16 mask) PnPos12 { get; protected set; }
        public UInt32 Pos32 { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            if (this.Pos32 != 0 && this.Pos32 == writ.POS32)
            {
                return this.NegatableFullMatch;
            }
            if (this.PnPos12.value != 0 && this.PnPos12.mask != 0 && this.PnPos12.value == (UInt16)(writ.pnPOS12 & this.PnPos12.mask))
            {
                return this.NegatableFullMatch;
            }
            return this.NegatableZeroMatch;
        }
        protected FeaturePartOfSpeech(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            this.PnPos12 = (0, 0);
            this.Pos32 = 0;
        }
    }
}