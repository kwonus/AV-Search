using AVSearch.Interfaces;
using AVXLib;

namespace AVSearch.Model.Types
{
    public abstract class TypeWildcard
    {
        public List<string> Contains { get; protected set; }
        public string? Beginning { get; protected set; }
        public string? Ending { get; protected set; }
        public List<string> ContainsHyphenated { get; protected set; }
        public string? BeginningHyphenated { get; protected set; }
        public string? EndingHyphenated { get; protected set; }
        public string Text { get; protected set; }

        protected TypeWildcard(string text)
        {
            this.Contains = new();
            this.ContainsHyphenated = new();
            this.Text = text;
        }

        public HashSet<UInt16> GetLexemes(ISettings settings)
        {
            var lexicon = ObjectTable.AVXObjects.Mem.Lexicon.Slice(1).ToArray();
            var lexemes = new HashSet<UInt16>();

            UInt16 key = 0;
            foreach (var lex in lexicon)
            {
                bool match = false;
                ++key;

                string kjv = lex.Display.ToString();
                string avx = lex.Modern.ToString();

                (bool normalized, bool hyphenated) kjvMatch = (false, false);
                (bool normalized, bool hyphenated) avxMatch = (false, false);

                bool hyphenated = kjv.Contains('-');

                string kjvNorm = hyphenated ? lex.Search.ToString() : kjv;
                string avxNorm = hyphenated ? lex.Search.ToString() : avx;  // transliterated names do not differ between kjv and avx

                kjvMatch.normalized = settings.UseLexiconAV
                    && ((this.Beginning == null) || kjvNorm.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                    && ((this.Ending == null) || kjvNorm.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                avxMatch.normalized = settings.UseLexiconAVX
                    && ((this.Beginning == null) || avxNorm.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                    && ((this.Ending == null) || avxNorm.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                match = kjvMatch.normalized || avxMatch.normalized;

                if (hyphenated)
                {
                    kjvMatch.hyphenated = settings.UseLexiconAV
                        && ((this.Beginning == null) || kjv.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                        && ((this.Ending == null) || kjv.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                    avxMatch.hyphenated = settings.UseLexiconAVX
                        && ((this.Beginning == null) || avx.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                        && ((this.Ending == null) || avx.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                    match = match || kjvMatch.hyphenated || avxMatch.hyphenated;
                }
                if (match && this.Contains.Count > 0)
                {
                    foreach (var piece in this.Contains)
                    {
                        if (kjvMatch.normalized)
                            kjvMatch.normalized = kjvNorm.Contains(piece, StringComparison.InvariantCultureIgnoreCase);
                        if (avxMatch.normalized)
                            avxMatch.normalized = avxNorm.Contains(piece, StringComparison.InvariantCultureIgnoreCase);

                        if (kjvMatch.hyphenated)
                            kjvMatch.hyphenated = kjv.Contains(piece, StringComparison.InvariantCultureIgnoreCase);
                        if (avxMatch.hyphenated)
                            avxMatch.hyphenated = avx.Contains(piece, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                if (kjvMatch.normalized || avxMatch.normalized || kjvMatch.hyphenated || avxMatch.hyphenated)
                {
                    if (!lexemes.Contains(key))
                        lexemes.Add(key);
                }
            }
            return lexemes;
        }
    }
}