namespace AVSearch.Model.Features
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;

    public abstract class FeatureTransition : FeatureGeneric
    {
        override public string Type { get => GetTypeName(this); }
        public byte Transition { get; protected set; }

        public override UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag)
        {
            if ((byte)(writ.Transition & this.Transition) == this.Transition)
            {
                return this.NegatableFullMatch;
            }
            return this.NegatableZeroMatch;
        }
        protected FeatureTransition(string text, bool negate, ISettings settings) : base(text, negate, settings)
        {
            this.Transition = 0;
        }
    }
}