
namespace AVSearch
{
    using Blueprint.Blue;
    public class TSettings
	{
		public TSettings(in QSettings settings)
		{
            this.Span = settings.Span.Value;
            this.Lexicon = (byte)(settings.Lexicon.Value);
            this.Similarity = settings.Similarity.Value;
            this.FuzzyLemmata = settings.Similarity.EnableLemmaMatching;
        }
		public bool FuzzyLemmata { get; private set; } 
		public byte Lexicon      { get; private set; }    // 1 | 2 | 3
		public byte Similarity   { get; private set; }    // 0 | 100 | 33 to 99
		public UInt16 Span       { get; private set; }    // 0 to 999
	}
}
