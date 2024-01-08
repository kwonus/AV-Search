namespace AVSearch.Model.Results
{
    using AVSearch.Model.Expressions;
    using Interfaces;
    using System;

    public class QueryResult
    {
        public QueryResult(IEnumerable<SearchExpression> expressions)
        {
            this.Expressions = new();
            foreach (var exp in expressions)
                this.Expressions.Add(exp);
            
            this.QueryId = Guid.NewGuid();
        }
        public byte BookCnt
        {
            get
            {
                byte cnt = 0;
                foreach (var exp in this.Expressions)
                {
                    foreach (var bk in exp.Books.Values)
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
                foreach (var exp in this.Expressions)
                {
                    foreach (var bk in exp.Books.Values)
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
                foreach (var exp in this.Expressions)
                {
                    foreach (var bk in exp.Books.Values)
                    {
                        if (bk.ChapterHits > 0)
                            hits += bk.ChapterHits;
                    }
                }
                return hits;
            }
        }
        public UInt32 ErrorCode { get; protected set; }
        public List<SearchExpression> Expressions { get; protected set; }
        public Guid QueryId { get; protected set; }
        public ulong TotalHits
        {
            get
            {
                UInt64 hits = 0;
                foreach (var exp in this.Expressions)
                {
                    foreach (var bk in exp.Books.Values)
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
                foreach (var exp in this.Expressions)
                {
                    foreach (var bk in exp.Books.Values)
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
