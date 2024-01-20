namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;
    using AVXLib;
    using AVXLib.Memory;

    public abstract class FeatureDelta : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            var entry = ObjectTable.AVXObjects.lexicon.GetRecord(writ.WordKey);
            bool delta = entry.valid && !LEXICON.IsModernSameAsDisplay(entry.entry);

            bool result = this.Negate ? !delta : delta;            
            return result ? FeatureGeneric.FullMatch : FeatureGeneric.ZeroMatch;
        }
        protected FeatureDelta(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            ;
        }
    }
}