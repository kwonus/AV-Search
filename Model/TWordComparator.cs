namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    class TWordComparator : TComparator
    {
        public List<UInt16> wkeys;
        public List<string> phonetics;
        public TWordComparator(ref QFeature feature) : base(ref feature)
        {
            this.wkeys = new();
            this.phonetics = new();
        }
        public override UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
    }
}