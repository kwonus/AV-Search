using System.Runtime.CompilerServices;

namespace AVSearch.Model.Expressions
{
    public abstract class SearchFilter
    {
        public abstract class RangeFilter
        {
            
        }
        public class ChapterRange : RangeFilter
        {
            public (byte from, byte to) Chapter;
            public ChapterRange(byte from, byte to)
            {
                this.Chapter = (from, to);
            }
        }
        public class VerseRange : RangeFilter
        {
            public byte Chapter;
            public (byte from, byte to) Verse;
            public VerseRange(byte chapter, byte from, byte to)
            {
                this.Chapter = chapter;
                this.Verse   = (from, to);
            }
        }
        public byte Book          { get; protected set; }
        public RangeFilter? Range { get; protected set; }
        protected SearchFilter()
        {
            this.Book = 0;
            this.Range = null;
        }
    }
}
