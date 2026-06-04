using System;

namespace Market.Domain.Entities
{
    public class ErrorLog : BaseEntity
    {
        public string Source { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string RequestPath { get; set; }
        public string HttpMethod { get; set; }
    }
}
