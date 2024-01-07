using AVSearch.Model.Expressions;
using AVSearch.Model.Results;
using AVSearch.Model.Types;

namespace AVSearch
{
    public class AVQueryManager
	{
		public AVQueryManager()
		{
			this.ClientQueries = new();
		}

		public Dictionary <Guid, Dictionary<Guid, QueryResult>> ClientQueries;

		public QueryResult? Create(in Guid client_id, in List<SearchExpression> expressions)
		{
			//var query = new TQuery(expressions);
			// query;
			return null;
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
