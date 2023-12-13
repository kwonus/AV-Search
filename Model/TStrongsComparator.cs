namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib.Framework;

	class TStrongsComparator : TComparator
	{
		public char lang     { get; private set; }
		public UInt64 number { get; private set; }
		public TStrongsComparator(ref QFeature feature) : base(ref feature)
		{
			;
		}
		public virtual UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
		{
			return 0;
		}
	}
}

