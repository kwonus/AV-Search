namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using Interfaces;
    using System;

    public class QueryResult
    {
        public QueryResult()
        {
            this.Expression = null;
            this.QueryId = Guid.Empty;
        }
        public QueryResult(SearchExpression expression)
        {
            this.Expression = expression;           
            this.QueryId = expression != null ? Guid.NewGuid() : Guid.Empty;
        }
        public byte BookCnt
        {
            get
            {
                byte cnt = 0;

                if (this.Expression != null)
                {
                    foreach (var bk in this.Expression.Books.Values)
                    {
                        if (bk.TotalHits > 0)
                            cnt++;
                    }
                }
                return cnt;
            }
        }
        public UInt64 BookHits
        {
            get
            {
                UInt64 hits = 0;

                if (this.Expression != null)
                {
                    foreach (var bk in this.Expression.Books.Values)
                    {
                        if (bk.TotalHits > 0)
                            hits ++;
                    }
                }
                return hits;
            }
        }
        public UInt64 ChapterHits
        {
            get
            {
                UInt64 hits = 0;

                if (this.Expression != null)
                {
                    foreach (var bk in this.Expression.Books.Values)
                    {
                        if (bk.ChapterHits > 0)
                            hits += bk.ChapterHits;
                    }
                }
                return hits;
            }
        }
        public UInt32 ErrorCode { get; protected set; }
        public SearchExpression? Expression { get; protected set; }
        public Guid QueryId { get; protected set; }
        public ulong TotalHits
        {
            get
            {
                UInt64 hits = 0;

                if (this.Expression != null)
                {
                    foreach (var bk in this.Expression.Books.Values)
                    {
                        if (bk.TotalHits > 0)
                            hits += bk.TotalHits;
                    }
                }
                return hits;
            }
        }
        public ulong VerseHits
        {
            get
            {
                UInt64 hits = 0;

                if (this.Expression != null)
                {
                    foreach (var bk in this.Expression.Books.Values)
                    {
                        foreach (var ch in bk.Chapters.Values)
                        {
                            if (ch.VerseHits > 0)
                                hits += ch.VerseHits;
                        }
                    }
                }
                return hits;
            }
        }
    }
}
