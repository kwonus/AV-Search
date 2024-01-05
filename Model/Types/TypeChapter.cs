namespace AVSearch.Model.Types
{
    using AVSearch.Model.Results;
    using System;

    public abstract class TypeChapter
    {
		public byte ChapterNum  { get => 0; }
		public UInt64 TotalHits { get; protected set; }
        public UInt64 VerseHits { get => 0; }

        public List<QueryMatch> Matches { get; protected set; }

        public void IncrementHits()
        {
            this.TotalHits++;
        }
    }
}