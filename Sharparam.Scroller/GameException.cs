namespace Sharparam.Scroller
{
    using System;
    using System.Runtime.Serialization;

    using log4net;

    public class GameException : Exception
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameException));

        public GameException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            Log.Error("New GameException created.", this);
        }

        protected GameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
