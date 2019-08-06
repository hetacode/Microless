using System;
namespace Hetacode.Microless.Attributes
{
    public class BindMessageAttribute : Attribute
    {
        public Type MessageType { get; }

        public BindMessageAttribute(Type messageType)
        {
            MessageType = messageType;
        }

    }
}
