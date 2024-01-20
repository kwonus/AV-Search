namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;
    using AVSearch.Model.Types;
    using PhonemeEmbeddings;

    public abstract class FeatureLexeme : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public HashSet<UInt16> WordKeys { get; protected set; }
         public Dictionary<string, Dictionary<UInt16, UInt16>> Phonetics { get; protected set; } //Dictionary<ipa, Dictionary<wordkey, matchscore>> // only includes entries where lexical and oov_lemma entries are above threshold
        public TypeWildcard? Wildcard { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            foreach (var lexeme in this.WordKeys)
            {
                if (lexeme == (writ.WordKey & 0X3FFF))
                {
                    return this.NegatableFullMatch;
                }
            }
            UInt16 maxSimilarity = 0;
            foreach (Dictionary<UInt16, UInt16> phones in this.Phonetics.Values)
            {
                if (phones.ContainsKey(writ.WordKey))
                {
                    UInt16 similarity = phones[writ.WordKey];
                    if (similarity > maxSimilarity)
                        maxSimilarity = similarity;
                }
            }
            return this.NegatableMatchScore(maxSimilarity);
        }
        protected FeatureLexeme(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            this.WordKeys = new();
            this.Phonetics = new();
            this.Wildcard = null;
        }
    }
}