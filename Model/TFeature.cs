namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;

    public class TFeature
	{
		public TFeature(ref TComparator comparator, UInt16 idx)
		{
			this.feature = comparator.Text;
			this.feature_idx = idx;
			this.feature_avx = comparator;
			this.hits = 0;
        }
		public string feature { get; private set; }
		public UInt16 feature_idx { get; private set; }
		public TComparator feature_avx { get; private set; }
		public UInt64 hits { get; private set; }
	}
}
