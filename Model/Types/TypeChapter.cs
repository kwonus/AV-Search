namespace AVSearch.Model.Types
{
    using AVSearch.Model.Results;
    using System;

    public abstract class TypeChapter
    {
		public byte ChapterNum  { get; protected set; }
		public UInt64 TotalHits { get; protected set; }
        public UInt64 VerseHits { get; protected set; }

        public List<QueryMatch> Matches { get; protected set; }
    }
}