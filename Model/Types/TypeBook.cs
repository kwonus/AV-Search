namespace AVSearch.Model.Types
{
    using AVSearch.Model.Results;
    using System;

    public abstract class TypeBook
    {
		public byte BookNum		    { get; protected set; }
		public byte ChapterCnt  	{ get; protected set; }
        public UInt64 ChapterHits 	{ get; protected set; }
        public UInt64 TotalHits   	{ get; protected set; }
        public UInt64 VerseHits     { get; protected set; }

        public Dictionary<UInt32, QueryMatch> Matches { get; protected set; }
        protected TypeBook()
        {
            this.Matches = new();
        }

        //      public Dictionary<byte, byte> VerseHitsByChapter { get; protected set; }
    }
}
