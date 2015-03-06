namespace Sharparam.Scroller
{
    using System;
    using System.Runtime.Serialization;

    using log4net;

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
