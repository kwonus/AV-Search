namespace AVSearch.Model.Results
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;
    using System;

    public class QueryChapter : TypeChapter
    {
        public byte ChapterNum { get; private set; }
        public QueryChapter(byte num, bool zeroHits = false) : base(num)
        {
            this.TotalHits = (UInt64) (zeroHits ? 0 : 1);
        }
        public bool AddScope(ScopingFilter range)
        {
            return false;
        }
        public override string Render(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            if (settings.RenderingFormat == ISettings.Formatting_JSON)
            {
                return this.RenderJSON(settings, scope);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_YAML)
            {
                return this.RenderYAML(settings, scope);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_TEXT)
            {
                return this.RenderTEXT(settings, scope);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_HTML)
            {
                return this.RenderHTML(settings, scope);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_MD)
            {
                return this.RenderMD(settings, scope);
            }
            return string.Empty;
        }
        private string RenderJSON(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderYAML(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderTEXT(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderHTML(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderMD(ISettings settings, IEnumerable<ScopingFilter>? scope)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
    }
}
