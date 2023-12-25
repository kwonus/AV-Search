namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using Interfaces;
    using System;

    public class QueryResult
    {
        protected QueryResult()
        {
            Expressions = new();
            QueryId = Guid.NewGuid();
        }
        public byte BookCnt { get; protected set; }
        public ulong BookHits { get; protected set; }
        public Dictionary<byte, QueryBook> Books { get; protected set; }
        public ulong ChapterHits { get; protected set; }
        public uint ErrorCode { get; protected set; }
        public List<SearchExpression> Expressions { get; protected set; }
        public Guid QueryId { get; protected set; }
        public ISettings Settings { get; protected set; }
        public ulong TotalHits { get; protected set; }
        public ulong VerseHits { get; protected set; }
        public List<uint> Scope { get; protected set; }

        public string Fetch(byte book, byte chapter)
        {
            return string.Empty;
        }

        public bool AddScope(uint spec)
        {
            byte book = (byte)(spec >> 24);

            if (book == 0)
            {
                for (byte num = 1; num <= 66; num++)
                {
                    Books[num] = new QueryBook(num);
                }
                BookCnt = 66;
                return true;
            }
            else if (book >= 1 && book <= 66)
            {
                if (!Books.ContainsKey(book))
                {
                    Books[book] = new QueryBook(book);
                }
                BookCnt = (byte)Books.Count;
                return true;
            }
            return false;
        }
        public bool Search()
        {
            if (BookCnt == 0)
                AddScope(0);

            int cnt = 0;
            bool ok = true;
            foreach (QueryBook book in Books.Values)
            {
                foreach (var expression in Expressions)
                {
                    cnt++;
                    //ok = book.search(ref expression, ref this.settings, ref this.scope); // TODO: update hits attributes in TQuery
                    if (!ok)
                        return false;
                }
            }
            return cnt > 0;
        }
    }
}
