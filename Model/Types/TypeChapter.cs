namespace AVSearch.Model.Types
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Results;
    using System;

    public abstract class TypeChapter
    {
		public byte ChapterNum  { get => 0; }
		public UInt64 TotalHits { get; protected set; }
        public UInt64 VerseHits { get => 0; }

        protected TypeChapter()
        {
            ;
        }

        public void IncrementHits()
        {
            this.TotalHits++;
        }

        public abstract string Render(ISettings settings, IEnumerable<ScopingFilter>? scope);
    }
}