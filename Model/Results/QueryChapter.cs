namespace AVSearch.Model.Results
{
    using AVSearch.Interfaces;
    using AVSearch.Model.Expressions;
    using AVSearch.Model.Types;
    using System;

    public class QueryChapter : TypeChapter
    {
        public QueryChapter(byte num) : base()
        {
            this.TotalHits = 1;
        }
        public bool AddScope(SearchFilter.ChapterRange range)
        {
            return false;
        }
        public override string Render(ISettings settings, SearchFilter.RangeFilter range)
        {
            if (settings.RenderingFormat == ISettings.Formatting_JSON)
            {
                return this.RenderJSON(settings, range);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_YAML)
            {
                return this.RenderYAML(settings, range);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_TEXT)
            {
                return this.RenderTEXT(settings, range);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_HTML)
            {
                return this.RenderHTML(settings, range);
            }
            else if (settings.RenderingFormat == ISettings.Formatting_MD)
            {
                return this.RenderMD(settings, range);
            }
            return string.Empty;
        }
        private string RenderJSON(ISettings settings, SearchFilter.RangeFilter range)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderYAML(ISettings settings, SearchFilter.RangeFilter range)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderTEXT(ISettings settings, SearchFilter.RangeFilter range)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderHTML(ISettings settings, SearchFilter.RangeFilter range)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
        private string RenderMD(ISettings settings, SearchFilter.RangeFilter range)
        {
            bool modernize = settings.RenderAsAVX;
            return string.Empty;
        }
    }
}
