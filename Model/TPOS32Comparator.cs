namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public class TPOS32Comparator : TComparator
    {
	    public UInt32 pos32 { get; private set; }
	    public TPOS32Comparator(ref QFeature feature): base(ref feature)
        {
            ;
        }
	    public virtual UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}

