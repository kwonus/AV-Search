namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public class TPuncComparator : TComparator
    {
        public byte punc { get; private set; }
        public TPuncComparator(ref QFeature feature) : base(ref feature)
        {
            ;
        }
        public override UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}

