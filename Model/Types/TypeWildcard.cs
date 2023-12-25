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
    }
}