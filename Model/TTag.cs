using System;

namespace AVSearch
{
    using Blueprint.Blue;

    public class TTag
    {
        public UInt32 Coordinates { get; private set; }
        public TOptions Options   { get; private set; }
        public TFeature Feature   { get; private set; }
 
        public TTag(TOptions options, TFeature feature, UInt32 coordinates = 0)
        {
            this.Options = options;
            this.Feature = feature;
            this.Coordinates = Coordinates;
        }
    
        byte get_book()
        {
            return (byte)(Coordinates & 0xFF00 >> 24);
        }
        byte get_chapter()
        {
            return (byte)(0xFF & (Coordinates >> 16));
        }
        byte get_verse()
        {
            return (byte)(0xFF & (Coordinates >> 8));
        }
        byte word()
        {
            return (byte)(0xFF & Coordinates);
        }
        static UInt32 CreateCoordinate(byte b, byte c, byte v, byte w)
        {
            return ((UInt32) b << 24) | ((UInt32) c << 16) | ((UInt32) v << 8) | (UInt32) w;
        }
    }
}
