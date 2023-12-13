namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;

	class TLemmaComparator : TComparator
	{
		public List<UInt16> lemmata;
		public TLemmaComparator(ref QFeature feature) : base(ref feature)
		{
			this.lemmata = new();
		}
		public override UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
		{
			return 0;
		}
	}
}
