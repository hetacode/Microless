using System;
namespace Contracts
{
    public class MessageResponse
    {
        public Guid CorrelationId { get; set; }

        public DateTime Time { get; set; }
    }
}
