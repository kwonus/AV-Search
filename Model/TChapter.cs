namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;


	public class TChapter
	{
		public TChapter(byte num)
		{
			this.matches = new();
		}
		public byte chapter_num  { get; private set; }
		public UInt64 total_hits { get; private set; }
        public UInt64 verse_hits { get; private set; }

        public List<TMatch> matches { get; private set; }
    }
}
