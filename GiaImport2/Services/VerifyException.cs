using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace GiaImport2.Services
{
    [Serializable]
    public class VerifyException : Exception
    {
        public ConcurrentDictionary<string, string> errorDict;

        public VerifyException()
        {
        }

        public VerifyException(string message) : base(message)
        {
        }

        public VerifyException(ConcurrentDictionary<string, string> errorDict)
        {
            this.errorDict = errorDict;
        }

        public VerifyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VerifyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}