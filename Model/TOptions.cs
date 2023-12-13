namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;
	public class TOptions
	{
		public TOptions(QMatchAny match_any, UInt16 match_any_idx)
		{
			this.options_avx = match_any;

			//this.options = match_any.options;
			this.options_idx = match_any_idx;
			UInt32 idx = 0;/*
			foreach (var feature in match_any.AnyFeature)
			{
				this.any_of.Add(new TFeature(feature, idx++));
			}*/
		}
		public List<TFeature> any_of { get; private set; }
		public string options		 { get; private set; }
		public UInt16 options_idx	 { get; private set; }
		public QMatchAny options_avx { get; private set; }
		public UInt64 hits			 { get; private set; }
	}
}