using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace GiaImport2.Services
{
    [Serializable]
    public class BulkException : Exception
    {
        public ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errorDict;

        public BulkException()
        {
        }

        public BulkException(string message) : base(message)
        {
        }

        public BulkException(ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errorDict)
        {
            this.errorDict = errorDict;
        }

        public BulkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BulkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}