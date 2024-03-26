namespace AVSearch.Model.Types
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Results;
    using System;

    public abstract class TypeChapter
    {
		public byte ChapterNum  { get; protected set; }
		public UInt64 TotalHits { get; protected set; }
        public UInt64 VerseHits { get => 0; }

        protected TypeChapter()
        {
            ;
        }
        protected TypeChapter(byte num)
        {
            this.ChapterNum = num;
        }

        public void IncrementHits()
        {
            this.TotalHits++;
        }

        public abstract string Render(ISettings settings, IEnumerable<ScopingFilter>? scope);
    }
}