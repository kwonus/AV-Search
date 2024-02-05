namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;
    using AVXLib;
    using AVXLib.Memory;

    public abstract class FeatureEntity : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public UInt16 Entity { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            (Lexicon entry, bool valid) record = ObjectTable.AVXObjects.lexicon.GetRecord(writ.WordKey);

            if (record.valid && ((UInt16)(this.Entity & record.entry.Entities) != 0))
            {
                return this.NegatableFullMatch;
            }
            return this.NegatableZeroMatch;
        }
        protected FeatureEntity(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            ;
        }
    }
}