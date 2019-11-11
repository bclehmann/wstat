using System;
using System.Runtime.Serialization;

namespace Where1.wstat
{
    [Serializable]
    internal class DimensionMismatchException : Exception
    {
        public DimensionMismatchException()
        {
            new DimensionMismatchException("An attempt was made to pair dimensions of differing lengths.");
        }

        public DimensionMismatchException(string message) : base(message)
        {
        }

        public DimensionMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DimensionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}