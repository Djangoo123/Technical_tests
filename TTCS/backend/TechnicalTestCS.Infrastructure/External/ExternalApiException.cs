using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalTestCS.Infrastructure.External
{
    public sealed class ExternalApiException : Exception
    {
        public int StatusCode { get; }

        public ExternalApiException(int statusCode, string message) : base(message)
            => StatusCode = statusCode;
    }
}
