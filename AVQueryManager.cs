namespace AVSearch
{
	using Blueprint.Blue;
	public class AVQueryManager
	{
		public AVQueryManager()
		{
			;
		}

		public Dictionary <Guid, Dictionary<Guid, TQuery>> ClientQueries;

		public TQuery Create(in Guid client_id, in List<QFind> expressions)
		{
			var query = new TQuery(expressions);
			return query;
		}
        public bool ReleaseAll(in Guid client_id)
        {
			var client = ClientQueries[client_id];
			if (client != null)
			{
				client.Clear();
                ClientQueries.Remove(client_id);
				return true;
            }
			return false;
        }
        public bool ReleaseQuery(in Guid client_id, in Guid query_id)
        {
            var client = ClientQueries[client_id];
            if ((client != null) && client.ContainsKey(query_id))
			{
				client.Remove(query_id);
				return true;
			}
            return false;
        }
	}
}
