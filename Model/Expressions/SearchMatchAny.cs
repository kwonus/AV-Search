namespace AVSearch.Model.Expressions
{
    using AVSearch.Model.Features;
    using AVSearch.Model.Types;
    using AVXLib.Framework;

    public abstract class SearchMatchAny
    {
        public abstract List<FeatureGeneric> AnyFeature { get; protected set; }
//      public abstract UInt16 Compare(Written written);
    }
}