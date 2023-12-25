namespace AVSearch.Interfaces
{
    using System;

    public interface ISettings
    {
        bool EnableFuzzyLemmata { get; }
        bool UseLexiconAV { get; }
        bool UseLexiconAVX { get; }
        byte SearchSimilarity { get; }  // 0 | 100 | 33 to 99
        ushort SearchSpan { get; }  // 0 to 999
    }
}
