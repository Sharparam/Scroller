namespace Sharparam.Scroller
{
    using System;
    using System.Runtime.Serialization;

    public class TilesetException : GameException
    {
        public TilesetException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }

        protected TilesetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
