namespace AVSearch
{
	using Blueprint.Blue;
	public class AVQueryManager
	{
		public AVQueryManager()
		{
			;
		}

		public Dictionary <UInt64, TQuery> queries;

		public TQuery? create(Blueprint blueprint)
		{
			return null;
		}
		public bool add_scope(UInt64 query_id, byte book, byte chapter, byte verse_from, byte verse_to)
		{
			return false;
		}
		public string fetch(UInt64 query_id)
		{
			return string.Empty;
		}
		public string fetch(UInt64 query_id, byte book, byte chapter)
		{
			return string.Empty;
		}
	}
}
