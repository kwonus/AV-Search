namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;

    public abstract class FeatureStrongs : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public (UInt16 number, char lang) Strongs { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            if (this.Strongs.lang == 'H' && writ.BCVWc.B > 39)
            {
                return FeatureGeneric.ZeroMatch;
            }
            if (this.Strongs.lang == 'G' && writ.BCVWc.B < 40)
            {
                return FeatureGeneric.ZeroMatch;
            }
            for (int n = 0; n < 4; n++)
            {
                UInt16 num = writ.Strongs[n];
                if (num == 0)
                    break;
                if (num == this.Strongs.number)
                    return this.NegatableFullMatch;
            }
            return this.NegatableZeroMatch;
        }
        protected FeatureStrongs(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            this.Strongs = (0, 'X');
        }
    }
}
