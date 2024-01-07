namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;
    using AVSearch.Model.Types;

    public abstract class FeatureLexeme : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public HashSet<UInt16> WordKeys { get; protected set; }
        public HashSet<string> Phonetics { get; protected set; }
        public TypeWildcard? Wildcard { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            foreach (var lexeme in this.WordKeys)
            {
                if (lexeme == (writ.WordKey & 0X3FFF))
                {
                    return FeatureGeneric.FullMatch;
                }
            }
            UInt16 MaxSimilarity = 0;
            foreach (var phone in this.Phonetics)
            {
                ;
            }
            return MaxSimilarity;
        }
        protected FeatureLexeme(string text, bool negate) : base(text, negate)
        {
            this.WordKeys = new();
            this.Phonetics = new();
            this.Wildcard = null;
        }
    }
}