using System;
namespace Service.Core.Attributes
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
