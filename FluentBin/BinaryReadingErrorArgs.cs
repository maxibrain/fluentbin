using System;

namespace FluentBin
{
    public class BinaryReadingErrorArgs : EventArgs
    {
        public BinaryReadingErrorArgs(BinaryReadingException exception)
        {
            Exception = exception;
        }

        public BinaryReadingException Exception { get; private set; }
        public bool Handled { get; set; }
    }
}