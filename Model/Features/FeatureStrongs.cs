namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureStrongs : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public (UInt16 number, char lang) Strongs { get; protected set; }

        public override UInt16 Compare(ref AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            return 0;
        }
        protected FeatureStrongs(string text, bool negate) : base(text, negate)
        {
            this.Strongs = (0, 'X');
        }
    }
}
