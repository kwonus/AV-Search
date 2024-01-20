using AVSearch.Interfaces;
using AVXLib;
using AVXLib.Memory;

namespace AVSearch.Model.Types
{
    public enum WildcardType
    {
        EnglishTerm = 0,
        NuphoneTerm = 1,
    }
    public abstract class TypeWildcard
    {
        public List<string> Contains { get; protected set; }
        public string? Beginning { get; protected set; }
        public string? Ending { get; protected set; }
        public List<string> ContainsHyphenated { get; protected set; }
        public string? BeginningHyphenated { get; protected set; }
        public string? EndingHyphenated { get; protected set; }
        public string Text { get; protected set; }
        public WildcardType TermType { get; protected set; }

        protected TypeWildcard(string text, WildcardType type)
        {
            this.Contains = new();
            this.ContainsHyphenated = new();
            this.Text = text;
            this.TermType = type;
        }
        public HashSet<UInt16> GetLexemes(ISettings settings)
        {
            return this.TermType == WildcardType.EnglishTerm
                ? this.GetLexemesFromEnglishWildcard(settings)
                : this.GetLexemesFromPhoneticWildcard(settings);
        }
        public HashSet<UInt16> GetLexemesFromEnglishWildcard(ISettings settings)
        {
            var lexicon = ObjectTable.AVXObjects.Mem.Lexicon.Slice(1).ToArray();
            var lexemes = new HashSet<UInt16>();

            UInt16 key = 0;
            foreach (var lex in lexicon)
            {
                bool match = false;
                ++key;

                string kjv = LEXICON.ToDisplayString(lex);
                string avx = LEXICON.ToModernString(lex);

                (bool normalized, bool hyphenated) kjvMatch = (false, false);
                (bool normalized, bool hyphenated) avxMatch = (false, false);

                bool hyphenated = LEXICON.IsHyphenated(lex);

                string kjvNorm = hyphenated ? LEXICON.ToSearchString(lex) : kjv;
                string avxNorm = hyphenated ? kjvNorm : avx;  // transliterated names (i.e. entries with hyphens) do not differ between kjv and avx

                kjvMatch.normalized = settings.SearchAsAV
                    && ((this.Beginning == null) || kjvNorm.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                    && ((this.Ending == null) || kjvNorm.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                avxMatch.normalized = settings.SearchAsAVX
                    && ((this.Beginning == null) || avxNorm.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                    && ((this.Ending == null) || avxNorm.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                match = kjvMatch.normalized || avxMatch.normalized;

                if (hyphenated)
                {
                    kjvMatch.hyphenated = settings.SearchAsAV
                        && ((this.Beginning == null) || kjv.StartsWith(this.Beginning, StringComparison.InvariantCultureIgnoreCase))
                        && ((this.Ending == null) || kjv.EndsWith(this.Ending, StringComparison.InvariantCultureIgnoreCase));

                    avxMatch.hyphenated = settings.SearchAsAVX
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

        public HashSet<UInt16> GetLexemesFromPhoneticWildcard(ISettings settings)
        {
            var lexicon = ObjectTable.AVXObjects.Mem.Lexicon.Slice(1).ToArray();
            var lexemes = new HashSet<UInt16>();

            return lexemes;
        }
    }
}