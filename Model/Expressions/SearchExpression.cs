namespace AVSearch.Model.Expressions
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Results;

    public abstract class SearchExpression
    {
        public bool Quoted          { get; protected set; }
        public string Expression    { get; protected set; }
        public UInt16 ExpressionIdx { get; protected set; }
        public List<SearchFragment> Fragments { get; protected set; }

        public ISettings Settings   { get; protected set; }
        public Dictionary<string, SearchFilter> Scope { get; protected set; }
        public bool Valid           { get; protected set; }
        public Dictionary<byte, QueryBook> Books { get; protected set; }

        protected SearchExpression()
        {
            this.Quoted = false;
            this.Expression = string.Empty;
            this.ExpressionIdx = 0;
            this.Fragments = new();
            this.Valid = false;
            this.Books = new();
            this.Scope = new();
        }

        public bool AddScope(byte book)
        {
            if (book == 0)
            {
                this.Books.Clear();
                for (byte num = 1; num <= 66; num++)
                {
                    this.Books[num] = new QueryBook(num);
                }
                return true;
            }
            else if (book >= 1 && book <= 66)
            {
                if (!Books.ContainsKey(book))
                {
                    this.Books[book] = new QueryBook(book);
                }
                return true;
            }
            return false;
        }

        public bool AddScope(SearchFilter filter)
        {
            byte book = filter.Book;

            if (book >= 1 && book <= 66)
            {
                if (!Books.ContainsKey(book))
                {
                    Books[book] = new QueryBook(book);
                }
                if (filter.Range != null)
                {
                    Books[book].AddRestrictiveRange(filter.Range);
                }
                return true;
            }
            return false;
        }
    }
}
