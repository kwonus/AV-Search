namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;

	public class TTransitionComparator : TComparator
	{
		public byte Tran { get; private set; }
		TTransitionComparator(ref QFeature feature) : base(ref feature)
		{
			;
		}
		public override UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
		{
			return 0;
		}
	}
}

