namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public interface IComparator
    {
        abstract UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag);
    }
    public class TComparator: IComparator, IFeature
    {
        private TComparator(ref QFeature feature, bool bad)
        {
            this.Okay = false;
        }
        protected TComparator(ref QFeature feature)
        {
            this.Okay = true;
        }
        public static TComparator Create(ref QFeature feature)
        {
            if (feature.Type.Equals("Word", StringComparison.InvariantCultureIgnoreCase) || feature.Type.Equals("Wildcard", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TWordComparator(ref feature);
            }
            if (feature.Type.Equals("PartOfSpeech", StringComparison.InvariantCultureIgnoreCase))
            {
                if (((QPartOfSpeech)feature).Pos32 != 0)
                    return new TPOS32Comparator(ref feature);
                else
                    return new TPOS16Comparator(ref feature);
            }
            if (feature.Type.Equals("Lemma", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TLemmaComparator(ref feature);
            }
            if (feature.Type.Equals("Delta", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TDeltaComparator(ref feature);
            }
            if (feature.Type.Equals("Punctuation", StringComparison.InvariantCultureIgnoreCase) || feature.Type.Equals("Decoration", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TPuncComparator(ref feature);
            }
            if (feature.Type.Equals("Strongs", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TStrongsComparator(ref feature);
            }
            if (feature.Type.Equals("Transition", StringComparison.InvariantCultureIgnoreCase))
            {
                return new TTransitionComparator(ref feature);
            }
            return new TComparator(ref feature, false); // comparisons are ALWAYS false in the base-class; this is a fail-safely error condition
        }

        public virtual UInt16 compare(ref AVXLib.Framework.Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
        public bool Okay        { get; private set;   }
        public string Type      { get; protected set; }
        public string Text      { get; protected set; }
        public bool Negate      { get; protected set; }
                              
        public QFeature Feature { get; protected set; }
        public Written Writ     { get; protected set; }
        public TMatch Match     { get; protected set; }
        public TTag Tag         { get; protected set; }
                              
        const UInt16 FullMatch = 1000;  // 100%
    }
}

