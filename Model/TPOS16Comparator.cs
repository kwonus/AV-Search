namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public class TPOS16Comparator : TComparator
    {
        public UInt16 pos16 { get; private set; }
        public TPOS16Comparator(ref QFeature feature) : base(ref feature)
        {
            ;
        }
        public virtual UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}