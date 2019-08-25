using System;
using Hetacode.Microless.Enums;

namespace Hetacode.Microless.Attributes
{
    public class HttpRequestAttribute : Attribute
    {
        public Type RequestType { get; private set; }
        public string Endpoint { get; private set; }
        public HttpMethod Method { get; private set; }
        public string QueueName { get; private set; }

        public HttpRequestAttribute(string endpoint, string queueName, HttpMethod method, Type requestType)
            => (Endpoint, QueueName, Method, RequestType) = (endpoint, queueName, method, requestType);
    }
}
