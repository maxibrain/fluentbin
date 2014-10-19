using System;

namespace FluentBin
{
    public class BinaryReadingAssertException : BinaryReadingException
    {
        public BinaryReadingAssertException(object o, object value, string member)
            : base(o, string.Format("Assertion failed during reading {0}", member))
        {
            Value = value;
        }

        public BinaryReadingAssertException(object o, object value, params object[] assertValues)
            : base(o, "Assertion failed")
        {
            AssertValues = assertValues;
            Value = value;
        }

        public object Value { get; private set; }
        public object[] AssertValues { get; private set; }
    }
}