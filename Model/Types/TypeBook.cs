namespace AVSearch.Model.Types
{
    using System;

    public abstract class TypeBook
    {
		public byte BookHits		{ get; protected set; }
		public byte BookNum		    { get; protected set; }
		public byte ChapterCnt  	{ get; protected set; }
        public UInt64 ChapterHits 	{ get; protected set; }
        public UInt64 TotalHits   	{ get; protected set; }
        public UInt64 VerseHits     { get; protected set; }

        public Dictionary<byte, byte> VerseHitsByChapter { get; protected set; }
    }
}
