using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Domain.Exceptions
{
    public class ExternalApiException : Exception
    {
        public string[]? Errors { get; set; }
        
        public ExternalApiException(string message) : base(message)
        {
        }
        
        public ExternalApiException(string message, string[] errors) : base(message)
        {
            Errors = errors;
        }

        public ExternalApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ExternalApiException(string message, string[] errors, Exception innerException) : base(message, innerException)
        {
            Errors = errors;
        }
    }
}