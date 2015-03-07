namespace Sharparam.Scroller.Mapping
{
    using System;
    using System.Runtime.Serialization;

    public class MapLoadException : GameException
    {
        public MapLoadException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }

        protected MapLoadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
