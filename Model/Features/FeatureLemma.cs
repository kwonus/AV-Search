namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureLemma : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public HashSet<UInt16> Lemmata { get; protected set; }
        public HashSet<string> Phonetics { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            foreach (var lexeme in this.Lemmata)
            {
                if (lexeme == (writ.Lemma & 0X3FFF))
                {
                    return this.NegatableFullMatch;
                }
            }
            UInt16 maxSimilarity = 0;
            foreach (var phone in this.Phonetics)
            {
                ;
            }
            return this.NegatableMatchScore(maxSimilarity);
        }
        protected FeatureLemma(string text, bool negate) : base(text, negate)
        {
            this.Lemmata = new();
            this.Phonetics = new();
        }
    }
}