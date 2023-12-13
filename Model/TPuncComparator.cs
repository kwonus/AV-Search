namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public class TPuncComparator : TComparator
    {
        public byte punc { get; private set; }
        TPuncComparator(ref QFeature feature) : base(ref feature)
        {
            ;
        }
        public virtual UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}

