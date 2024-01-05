namespace AVSearch.Model.Features
{
    using AVSearch.Model.Results;

    public abstract class FeatureGeneric
    {
        public abstract string Type { get; }
        public string Text { get; protected set; }
        public bool Negate { get; protected set; }
        public abstract UInt16 Compare(AVXLib.Memory.Written writ, ref QueryMatch match, ref QueryTag tag);

        protected static string GetTypeName(object obj)
        {
            string name = obj.GetType().Name;
            return name.Length >= 8 && name.StartsWith("Feature") ? name.Substring(7) : name;
        }
        protected FeatureGeneric(string text, bool negate)
        {
            this.Text = text.Trim();
            this.Negate = negate;

            if (this.Negate && this.Text.StartsWith('-'))
            {
                this.Text = this.Text.Length > 1 ? this.Text.Substring(1) : string.Empty;
            }
        }
        public const UInt16 FullMatch = 1000;  // 100%
    }
}
