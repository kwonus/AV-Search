namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;
    using static System.Formats.Asn1.AsnWriter;

    public abstract class FeatureGeneric
    {
        public abstract string Type { get; }
        public string Text { get; protected set; }
        public bool Negate { get; protected set; }
        public UInt64 Hits { get; private set; }
        protected ISettings Settings { get; set; }
        public abstract UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag);

        protected static string GetTypeName(object obj)
        {
            string name = obj.GetType().Name;
            return name.Length >= 8 && name.StartsWith("Feature") ? name.Substring(7) : name;
        }
        protected FeatureGeneric(string text, bool negate, ISettings settings)
        {
            this.Hits = 0;
            this.Text = text.Trim();
            this.Negate = negate;
            this.Settings = settings;

            if (this.Negate && this.Text.StartsWith('-'))
            {
                this.Text = this.Text.Length > 1 ? this.Text.Substring(1) : string.Empty;
            }
        }
        public void IncrementHits()
        {
            this.Hits++;
        }
        public UInt16 NegatableScore(UInt16 score)
        {
            if (score > FeatureGeneric.FullMatch)
                return 0;

            return !this.Negate ? score : (UInt16)(FullMatch - score);

        }
        public UInt16 NegatableFullMatch
        {
            get
            {
                return !this.Negate ? FeatureGeneric.FullMatch : FeatureGeneric.ZeroMatch;
            }
        }
        public UInt16 NegatableZeroMatch
        {
            get
            {
                return !this.Negate ? FeatureGeneric.ZeroMatch : FeatureGeneric.FullMatch;
            }
        }
        public UInt16 NegatableMatchScore(UInt16 score)
        {
            if (score > FeatureGeneric.FullMatch)
                return 0;

            return !this.Negate ? score : (UInt16)(FullMatch - score);

        }
        public const UInt16 FullMatch = 1000;  // 100%
        public const UInt16 ZeroMatch =    0;  //   0%
    }
}
