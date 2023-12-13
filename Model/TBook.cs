namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;

	public class TBook
	{
		public TBook(byte num)
		{

		}
		public byte book_hits		{ get; private set; }
		public byte book_num		{ get; private set; }
		public byte chapter_cnt  	{ get; private set; }
        public UInt64 chapter_hits 	{ get; private set; }
        public UInt64 total_hits   	{ get; private set; }
        public UInt64 verse_hits    { get; private set; }

		public Dictionary<byte, UInt64> verse_hits_by_chapter;
		public string fetch(byte chapter_num)
		{
			return string.Empty;
		}
		public bool search(ref TExpression expression, ref TSettings settings, ref List<UInt32> scope)
		{
            return expression.Quoted
                ? search_quoted(ref expression, ref settings, ref scope)
                : search_unquoted(ref expression, ref settings, ref scope);
        }
		private bool search_quoted(ref TExpression expression, ref TSettings settings, ref List<UInt32> scope)
        {
			return false;
		}
		private bool search_unquoted(ref TExpression expression, ref TSettings settings, ref List<UInt32> scope)
        {
			return false;
		}
		private Dictionary<byte, TChapter> chapters;

	}
}