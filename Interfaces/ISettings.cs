namespace AVSearch.Interfaces
{
    using System;

    public interface ISettings
    {
        bool SearchAsAV { get; }
        bool SearchAsAVX { get; }
        bool RenderAsAV { get; }
        bool RenderAsAVX { get; }
        int RenderingFormat { get; }
        (byte word, byte lemma) SearchSimilarity { get; }  // 0 | 100 | 33 to 99
        ushort SearchSpan { get; }  // 0 to 999

        const int Formatting_JSON = 0;
        const int Formatting_YAML = 1;
        const int Formatting_TEXT = 2;
        const int Formatting_HTML = 3;
        const int Formatting_MD   = 4;

        const int Lexion_UNDEFINED = 0;
        const int Lexion_AV = 1;
        const int Lexion_AVX = 2;
        const int Lexion_BOTH = 3;
    }
}
