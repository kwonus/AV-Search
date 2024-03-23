using AVXLib;
using AVXLib.Memory;
using System.Runtime.CompilerServices;

namespace AVSearch.Model.Expressions
{
    public class ScopingFilter
    {
        private byte ChapterFrom;
        private byte ChapterTo;
        public byte  Book        { get; private set; }
        public Book  Details     { get => ObjectTable.AVXObjects.Mem.Book.Slice(this.Book, 1).Span[0];  }
        private HashSet<byte>?   AmmendedChapters;
        public virtual IEnumerable<byte> Chapters
        {
            get
            {
                if (this.ChapterFrom > 0 && this.ChapterTo >= this.ChapterFrom)
                    for (byte c = this.ChapterFrom; c <= this.ChapterTo; c++)
                        yield return c;

                else if (this.AmmendedChapters != null)
                    foreach (byte c in from chapter in this.AmmendedChapters orderby chapter ascending select chapter)
                        yield return c;
            }
        }
        public virtual bool isCompleteBook
        {
            get
            {
                return this.ChapterFrom == 1 && this.ChapterTo == ObjectTable.AVXObjects.Mem.Book.Slice(this.Book, 1).Span[0].chapterCnt;
            }
        }
        private ScopingFilter(byte book, byte cfrom = 0, byte cto = 0)
        {
            byte chapterCnt = ObjectTable.AVXObjects.Mem.Book.Slice(book, 1).Span[0].chapterCnt;
            this.Book = book;
            this.AmmendedChapters = null;

            if (cfrom >= 1 && cfrom <= chapterCnt)
                this.ChapterFrom = cfrom;
            else
                this.ChapterFrom = 1;

            if (cto >= this.ChapterFrom && cto <= chapterCnt)
                this.ChapterTo = cto;
            else
                this.ChapterTo = cto > this.ChapterFrom ? chapterCnt : this.ChapterFrom;          
        }
        public static ScopingFilter? Create(byte book, byte cfrom = 0, byte cto = 0)
        {
            if (book >= 1 &&  book <= 66)
            {
                return new(book, cfrom, cto);
            }
            return null;
        }
        public static ScopingFilter? CreateDiscreteScope(string spec)
        {
            string[] parts = spec.Split(' ');
            string bookpart = parts[0];
            byte cfrom = 0;
            byte cto   = 0;
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i][0] >= '0' && parts[i][0] <= '9')
                {
                    string nums = parts[i];
                    for (++i; i < parts.Length; i++)
                        nums += parts[i];
                    string[] numparts = parts[i].Split('-');
                    if (numparts.Length <= 2)
                        try
                        {
                            cfrom = byte.Parse(numparts[0]);
                            cto = cfrom;
                        }
                        catch
                        {
                            cfrom = 0;
                            cto = 0;
                        }
                    if (numparts.Length == 2)
                        try
                        {
                            cto = byte.Parse(numparts[0]);
                        }
                        catch
                        {
                            cto = cfrom;
                        }
                    break;
                }
                else
                {
                    bookpart += (" " + parts[i]);
                }                   
            }
            string unspaced = bookpart.Replace(" ", "");
            var books = ObjectTable.AVXObjects.Mem.Book.Slice(0, 67).Span;
            Book? book = null;
            for (int b = 1; b <= 66; b++)
            {
                string name = books[b].name.ToString();
                if (name.Equals(bookpart, StringComparison.InvariantCultureIgnoreCase))
                {
                    book = books[b];
                    break;
                }
                if (name.Replace(" ", "").Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                {
                    book = books[b];
                    break;
                }
            }
            if (book == null)
            {
                for (int b = 1; b <= 66; b++)
                {
                    string alt = books[b].abbr2.ToString();
                    if (alt.Length == 0)
                        continue;
                    if (alt.StartsWith(unspaced, StringComparison.InvariantCultureIgnoreCase))
                    {
                        book = books[b];
                        break;
                    }
                }
            }
            if (book == null)
            {
                for (int b = 1; b <= 66; b++)
                {
                    string alt = books[b].abbr3.ToString();
                    if (alt.Length == 0)
                        continue;
                    if (alt.StartsWith(unspaced, StringComparison.InvariantCultureIgnoreCase))
                    {
                        book = books[b];
                        break;
                    }
                }
            }
            if (book == null)
            {
                for (int b = 1; b <= 66; b++)
                {
                    string alt = books[b].abbr4.ToString();
                    if (alt.Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                    {
                        book = books[b];
                        break;
                    }
                }
            }
            if (book == null)
            {
                for (int b = 1; b <= 66; b++)
                {
                    string alt = books[b].abbrAlternates.ToString(); // at this point, we only handle the first alternate if it exists
                    if (alt.Length == 0)
                        continue;
                    if (alt.Equals(unspaced, StringComparison.InvariantCultureIgnoreCase))
                    {
                        book = books[b];
                        break;
                    }
                }
            }
            if (book.HasValue)
            {
                return new ScopingFilter(book.Value.bookNum, cfrom, cto);
            }
            return null;
        }
        public ScopingFilter? Ammend(ScopingFilter ammendment)
        {
            if (this.Book != ammendment.Book)
                return null;

            if (this.isCompleteBook)
                return this;
            if (ammendment.isCompleteBook)
                return ammendment;

            if (this.AmmendedChapters == null)
            {
                this.AmmendedChapters = new();
                foreach (byte b in this.Chapters)
                    this.AmmendedChapters.Add(b);

                this.ChapterFrom = 0;
                this.ChapterTo = 0;
            }
            foreach (byte b in ammendment.Chapters)
                if (!this.AmmendedChapters.Contains(b))
                    this.AmmendedChapters.Add(b);

            return this;
        }
        public static IEnumerable<ScopingFilter>? Create(string spec)
        {
            switch (spec.Trim().ToLower().Replace(" ", ""))
            {
                case "oldtestament":
                case "o.t.":
                case "ot":
                    for (byte i = 1; i <= 39; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "newtestament":
                case "n.t.":
                case "nt":
                    for (byte i = 40; i <= 66; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "law":
                case "pentateuch":
                    for (byte i = 1; i <= 5; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "history":
                    for (byte i = 6; i <= 17; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "wisdomandpoetry":
                case "wisdom&poetry":
                case "wisdom+poetry":
                case "wisdom":
                case "poetry":
                    for (byte i = 18; i <= 22; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "majorprophets":
                    for (byte i = 23; i <= 27; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "minorprophets":
                    for (byte i = 28; i <= 39; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "prophets":
                    for (byte i = 23; i <= 39; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "gospels":
                    for (byte i = 40; i <= 43; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "gospelsandacts":
                case "gospels^acts":
                case "gospels+acts":
                    for (byte i = 40; i <= 44; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "churchepistle":
                case "churchepistles":
                    for (byte i = 45; i <= 53; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "pastoralepistle":
                case "pastoralepistles":
                    for (byte i = 54; i <= 57; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "generalepistle":
                case "generalepistles":
                case "jewishepistle":
                case "jewishepistles":
                    for (byte i = 58; i <= 65; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "epistle":
                case "epistles":
                    for (byte i = 45; i <= 65; i++)
                        yield return new ScopingFilter(i);
                    goto done;

                case "generalepistlesandrevelation":
                case "generalepistles&revelation":
                case "generalepistles+revelation":
                case "generalepistleandrevelation":
                case "generalepistle&revelation":
                case "generalepistle+revelation":
                    for (byte i = 58; i <= 66; i++)
                        yield return new ScopingFilter(i);
                    goto done;
            }
            ScopingFilter filter = CreateDiscreteScope(spec);
            if (filter != null)
                yield return filter;
        done:
            ;
        }
    }
}
