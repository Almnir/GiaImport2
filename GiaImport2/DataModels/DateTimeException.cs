using System;
using System.Runtime.Serialization;

namespace GiaImport2.DataModels
{
    [Serializable]
    internal class DateTimeException : Exception
    {
        public DateTimeException()
        {
        }

        public DateTimeException(string message) : base(message)
        {
        }

        public DateTimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DateTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}