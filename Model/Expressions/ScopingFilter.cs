namespace AVSearch.Model.Expressions
{
    using AVXLib;
    using AVXLib.Memory;
    using System;

    public class ScopingFilter
    {
        public byte  Book        { get; private set; }
        public Book  GetDetails() => ObjectTable.AVXObjects.Mem.Book.Slice(this.Book, 1).Span[0];
        public HashSet<byte>     Chapters;
        public virtual IEnumerable<byte> GetOrderedChapters()
        {
            foreach (byte c in from chapter in this.Chapters orderby chapter ascending select chapter)
                yield return c;
        }
        public bool IsCompleteBook()
        {
            return this.Chapters.Count == (int) ObjectTable.AVXObjects.Mem.Book.Slice(this.Book, 1).Span[0].chapterCnt;
        }
        public ScopingFilter() // for deserialization
        {
            this.Book = 0;
            this.Chapters = new();
        }
        private ScopingFilter(byte book)
        {
            byte chapterCnt = ObjectTable.AVXObjects.Mem.Book.Slice(book, 1).Span[0].chapterCnt;
            this.Book = book;
            this.Chapters = new();

            for (byte c = 1; c <= chapterCnt; c++)
                this.Chapters.Add(c);
        }
        private ScopingFilter(byte book, byte chapter)
        {
            byte chapterCnt = ObjectTable.AVXObjects.Mem.Book.Slice(book, 1).Span[0].chapterCnt;
            this.Book = book;
            this.Chapters = [ chapter ];
        }
        private ScopingFilter(byte book, byte cfrom, byte cto)
        {
            byte chapterCnt = ObjectTable.AVXObjects.Mem.Book.Slice(book, 1).Span[0].chapterCnt;
            this.Book = book;
            this.Chapters = new();

            if (cfrom >= 1 && cfrom <= chapterCnt && cto >= cfrom && cto <= chapterCnt)
                for (byte c = cfrom; c <= cto; c++)
                    this.Chapters.Add(c);

            else
                for (byte c = 1; c <= chapterCnt; c++)
                    this.Chapters.Add(c);
        }

        public bool InScope(byte chapter)
        {
            return this.Chapters.Contains(chapter);
        }
        public static ScopingFilter? CreateDiscreteScope(string textual)
        {
            byte book = GetBookNum(textual);
            return book >= 1 && book <= 66 ? new ScopingFilter(book) : null;
        }
        private static IEnumerable<ScopingFilter> CreateBookFilters(string textual, ChapterRange[] ranges)
        {
            byte book = GetBookNum(textual);

            if (book >= 1 && book <= 66)
            {
                foreach (ChapterRange range in ranges)
                {
                    yield return range.Unto.HasValue
                        ? new ScopingFilter(book, range.From, range.Unto.Value)
                        : new ScopingFilter(book, range.From);
                }
            }
        }
        public static byte GetBookNum(string text)
        { 
            string unspaced = text.Replace(" ", "");
            var books = ObjectTable.AVXObjects.Mem.Book.Slice(0, 67).Span;

            for (byte b = 1; b <= 66; b++)
            {
                string name = books[b].name.ToString();
                if (name.Equals(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
                if (name.Replace(" ", "").Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
            }
            for (byte b = 1; b <= 66; b++)
            {
                string alt = books[b].abbr2.ToString();
                if (alt.Length == 0)
                    continue;
                if (alt.StartsWith(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
            }

            for (byte b = 1; b <= 66; b++)
            {
                string alt = books[b].abbr3.ToString();
                if (alt.Length == 0)
                    continue;
                if (alt.StartsWith(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
            }

            for (byte b = 1; b <= 66; b++)
            {
                string alt = books[b].abbr4.ToString();
                if (alt.Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
            }

            for (byte b = 1; b <= 66; b++)
            {
                string alt = books[b].abbrAlternates.ToString(); // at this point, we only handle the first alternate if it exists
                if (alt.Length == 0)
                    continue;
                if (alt.Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    return b;
                }
            }
            return 0;
        }
        public bool Ammend(ScopingFilter ammendment)
        {
            if (this.Book != ammendment.Book)
                return false;

            if (!this.IsCompleteBook())
            {
                foreach (byte c in ammendment.Chapters)
                    if (!this.Chapters.Contains(c))
                        this.Chapters.Add(c);
            }
            return true;
        }
        public static IEnumerable<ScopingFilter>? Create(string textual, ChapterRange[] ranges)
        {
            byte book = GetBookNum(textual);

            if (book >= 1 && book <= 66)
            {
                if (ranges.Length == 0)
                {
                    ScopingFilter? discreteFilter = CreateDiscreteScope(textual);
                    if (discreteFilter != null)
                        yield return discreteFilter;
                }
                else
                {
                    IEnumerable<ScopingFilter> filters = CreateBookFilters(textual, ranges);
                    foreach (ScopingFilter filter in filters)
                        yield return filter;
                }
            }
            else
            {
                switch (textual.Trim().ToLower().Replace(" ", ""))
                {
                    case "oldtestament":
                    case "o.t.":
                    case "ot":
                        for (byte i = 1; i <= 39; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "newtestament":
                    case "n.t.":
                    case "nt":
                        for (byte i = 40; i <= 66; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "law":
                    case "pentateuch":
                        for (byte i = 1; i <= 5; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "history":
                        for (byte i = 6; i <= 17; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "wisdomandpoetry":
                    case "wisdom&poetry":
                    case "wisdom+poetry":
                    case "wisdom":
                    case "poetry":
                        for (byte i = 18; i <= 22; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "majorprophets":
                        for (byte i = 23; i <= 27; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "minorprophets":
                        for (byte i = 28; i <= 39; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "prophets":
                        for (byte i = 23; i <= 39; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "gospels":
                        for (byte i = 40; i <= 43; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "gospelsandacts":
                    case "gospels&acts":
                    case "gospels+acts":
                        for (byte i = 40; i <= 44; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "churchepistle":
                    case "churchepistles":
                        for (byte i = 45; i <= 51; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "pastoralepistle":
                    case "pastoralepistles":
                        for (byte i = 52; i <= 57; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "generalepistle":
                    case "generalepistles":
                    case "jewishepistle":
                    case "jewishepistles":
                        for (byte i = 58; i <= 65; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "epistle":
                    case "epistles":
                        for (byte i = 45; i <= 65; i++)
                            yield return new ScopingFilter(i);
                        break;

                    case "generalepistlesandrevelation":
                    case "generalepistles&revelation":
                    case "generalepistles+revelation":
                    case "generalepistleandrevelation":
                    case "generalepistle&revelation":
                    case "generalepistle+revelation":
                        for (byte i = 58; i <= 66; i++)
                            yield return new ScopingFilter(i);
                        break;
                }
            }
        }
    }
}
