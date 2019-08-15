using System;
namespace Hetacode.Microless.Exceptions
{
    public class ErrorException : Exception
    {
        public object Error { get; set; }
    }
}
