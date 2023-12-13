namespace AVSearch
{
	using Blueprint.Blue;
	public class AVClientManager
	{
		public AVClientManager()
		{
			;this.clients = new();
		}

		Dictionary<Guid, TQuery> clients;

		TQuery? query_create(Guid client_id, Blueprint blueprint, UInt16 span, byte lexicon, byte similarity, byte fuzzy_lemmata)
		{
			return null;
		}
		bool query_scope_add(Guid client_id, UInt64 query_id, byte book, byte chapter, byte verse_from, byte verse_to)
		{
			return false;
		}

		// returns json of TQuery:
		string fetch(Guid client_id, UInt64 query_id)
		{
			return string.Empty;
		}

		// returns json of TChapter:
		string fetch(Guid client_id, UInt64 query_id, byte book, byte chapter)
        {
            return string.Empty;
        }

        void client_release(Guid client_guid)
		{
			;
		}
		void query_release(Guid client_guid, UInt64 query_id)
		{
			;
		}

	}
}
