using System;

namespace FluentBin
{
    public class BinaryReadingException : Exception
    {
        public BinaryReadingException(object o, string message = null, Exception innerException = null)
            : base(message ?? string.Format("Error occurred during reading {0}", o.GetType()), innerException)
        {
            Object = o;
        }

        public object Object { get; private set; }
    }
}