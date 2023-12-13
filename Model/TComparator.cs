namespace AVSearch
{
    using Blueprint.Blue;
    using AVXLib;
    using System;
    using AVXLib.Framework;
    using System.Xml.Linq;

    public interface IComparator
    {
        abstract UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag);
    }
    public class TComparator: IComparator, IFeature
    {
        private TComparator(ref QFeature feature, bool bad)
        {
            this.Okay = false;
        }
        protected TComparator(ref QFeature feature)
        {
            this.Okay = true;
        }
        public static TComparator Create(ref QFeature feature)
        {
            auto feature = node["Text"].GetString();

            if (feature != nullptr)
            {
                auto type = node["Type"].GetString();

                if (std::strncmp(type, "Word", 4) == 0 || std::strncmp(type, "Wildcard", 8) == 0)
                {
                    return new TWordComparator(node);
                }
                if (std::strncmp(type, "PartOfSpeech", 12) == 0)
                {
                    if (node.HasMember("PnPos12") && (node["PnPos12"].GetUint() > 0))
                        return new TPOS16Comparator(node);
                    if (node.HasMember("Pos32") && (node["Pos32"].GetUint() > 0))
                        return new TPOS32Comparator(node);

                    return new TComparator(node, false);
                }
                if (std::strncmp(type, "Lemma", 8) == 0)
                {
                    return new TLemmaComparator(node);
                }
                if (std::strncmp(type, "Delta", 8) == 0)
                {
                    return new TDeltaComparator(node);
                }
                if (std::strncmp(type, "Punctuation", 11) || std::strncmp(type, "Decoration", 10) == 0)
                {
                    return new TPuncComparator(node);
                }
                if (std::strncmp(type, "Strongs", 8) == 0)
                {
                    return new TStrongsComparator(node);
                }
                if (std::strncmp(type, "Transition", 8) == 0)
                {
                    return new TTransitionComparator(node);
                }
            }
            return new TComparator(ref feature, false); // comparisons are ALWAYS false in the base-class; this is a fail-safely error condition
        }

        public virtual UInt16 compare(ref Written writ, ref TMatch match, ref TTag tag)
        {
            return 0;
        }
        public bool Okay        { get; private set;   }
        public string Type      { get; protected set; }
        public string Text      { get; protected set; }
        public bool Negate      { get; protected set; }
                              
        public QFeature Feature { get; protected set; }
        public Written Writ     { get; protected set; }
        public TMatch Match     { get; protected set; }
        public TTag Tag         { get; protected set; }
                              
        const UInt16 FullMatch = 1000;  // 100%
    }
}

