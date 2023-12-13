namespace AVSearch
{
	using Blueprint.Blue;
	using AVXLib;
	using System;
	using AVXLib.Framework;
	public class TQuery
	{
		private Blueprint blueprint;

		public TQuery(ref Blueprint blueprint)
		{
			this.blueprint = blueprint;
		}
		public byte book_cnt					{ get; private set; }
		public UInt64 book_hits					{ get; private set; }
		public Dictionary<byte, TBook> books	{ get; private set; }
        public UInt64 chapter_hits				{ get; private set; }
		public UInt32 error_code				{ get; private set; }
		public List<TExpression> expressions	{ get; private set; }
        public UInt64 query_id					{ get; private set; }
		public TSettings settings				{ get; private set; }
        public UInt64 total_hits				{ get; private set; }
        public UInt64 verse_hits				{ get; private set; }
		public List<UInt32> scope				{ get; private set; }

		public string fetch(byte book, byte chapter)
		{
			return string.Empty;
		}

		public bool add_scope(UInt32 spec)
		{
            byte book = (byte) (spec >> 24);

            if (book == 0)
            {
                for (byte num = 1; num <= 66; num++)
                {
                    this.books[num] = new TBook(num);
                }
                this.book_cnt = 66;
                return true;
            }
            else if (book >= 1 && book <= 66)
            {
                if (!books.ContainsKey(book))
                {
                    this.books[book] = new TBook(book);
                }
                this.book_cnt = (byte) this.books.Count;
                return true;
            }
            return false;
        }
		public bool search()
		{
            if (this.book_cnt == 0)
                this.add_scope(0);

            int cnt = 0;
            bool ok = true;
            foreach (TBook book in this.books.Values)
            {
                foreach (var expression in this.expressions)
                {
                    cnt++;
                    //ok = book.search(ref expression, ref this.settings, ref this.scope); // TODO: update hits attributes in TQuery
                    if (!ok)
                        return false;
                }
            }
            return (cnt > 0);
        }
	}
}
