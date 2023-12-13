
namespace AVSearch
{
	using Blueprint.Blue;
	public class TSettings
	{
		public TSettings(ref QSettings settings)
		{

		}
		public byte fuzzy_lemmata { get; private set; } // 0 (false) or 1 (true)
		public byte lexicon { get; private set; }   // 1 | 2 | 3
		public byte similarity { get; private set; }    // 0 | 100 | 33 to 99
		public UInt16 span { get; private set; }    // 0 to 999
	}
}
