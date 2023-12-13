namespace AVSearch
{ 
	using Blueprint.Blue;

	public class TFragment
	{
		public TFragment(QFragment frag, UInt16 frag_idx)
		{
			;
		}
		public UInt64 hits {  get; private set; }
		public bool anchored { get; private set; }
		public string fragment  {  get; private set; }
		public UInt16 fragment_idx { get; private set; }
		public List<TOptions> all_of { get; private set; }
		public QFragment fragment_avx  {  get; private set; }
	}
}
