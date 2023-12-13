namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;
    public class TDeltaComparator : TComparator
    {
	    public bool delta { get; private set; }
	    TDeltaComparator(ref QFeature feature) : base(ref feature)
        {
            ;
        }
        public override UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}