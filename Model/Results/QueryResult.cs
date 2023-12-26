namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using Interfaces;
    using System;

    public class QueryResult
    {
        public QueryResult()
        {
            Expressions = new();
            QueryId = Guid.NewGuid();
        }
        public byte BookCnt { get; }
        public ulong BookHits { get; }
        public ulong ChapterHits { get; }
        public uint ErrorCode { get; protected set; }
        public List<SearchExpression> Expressions { get; protected set; }
        public Guid QueryId { get; protected set; }
        public ISettings Settings { get; protected set; }
        public ulong TotalHits { get; }
        public ulong VerseHits { get; }

        public bool Search(SearchExpression expression)
        {
            /*
            if (BookCnt == 0)
                AddScope(0);

            int cnt = 0;
            bool ok = true;
            foreach (QueryBook book in Books.Values)
            {
                cnt++;
                //ok = book.search(ref expression, ref this.settings, ref this.scope); // TODO: update hits attributes in TQuery
                if (!ok)
                    return false;
            }
            return cnt > 0;
            */
            return false;
        }
    }
}
