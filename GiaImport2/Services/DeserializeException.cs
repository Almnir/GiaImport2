using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace GiaImport2.Services
{
    [Serializable]
    public class DeserializeException : Exception
    {
        public ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errorDict;

        public DeserializeException()
        {
        }

        public DeserializeException(string message) : base(message)
        {
        }

        public DeserializeException(ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errorDict)
        {
            this.errorDict = errorDict;
        }

        public DeserializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeserializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}