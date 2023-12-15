namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;

	public class TExpression
	{
		public TExpression(in QFind exp, UInt16 exp_idx)
		{
			this.fragments = new();
			this.expression_idx = exp_idx;
		}
		public bool Quoted               { get; private set; }

		public List<TFragment> fragments { get; private set; }
		public string expression		 { get; private set; }
		public UInt16 expression_idx	 { get; private set; }

		public QFind expression_avx      { get; private set; }
	}
}
