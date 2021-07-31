using System;

namespace Lingua.ZoomIntegration
{
    public class ZoomClientException : Exception
    {
        public ZoomClientException(string message, string response = null) : base(message)
        {
            Response = response;
        }

        public string Response { get; }

        public override string ToString()
        {
            return $"{base.ToString()}{Environment.NewLine}response: {Response}";
        }
    }
}
