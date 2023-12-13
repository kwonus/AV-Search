namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;
	public class TMatch
	{
		public TMatch()
		{
			this.highlights = new();
		}

		public UInt32 start                        { get; private set; }
        public UInt32 until						   { get; private set; }

		public bool Add(ref TTag match)
		{
			return false;
		}

		public List<TTag> highlights		       { get; private set; }
		public TExpression expression			   { get; private set; }
		public TFragment fragment				   { get; private set; }

		public TQuery? find()
		{
			return null;
		}
	}
}
