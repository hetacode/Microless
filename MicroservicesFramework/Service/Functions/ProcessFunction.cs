using System;
using System.Threading.Tasks;
using Contracts;
using Service.Core;
using Service.Core.Attributes;

namespace Service.Functions
{
    public class ProcessFunction
    {
        [BindMessage(typeof(MessageRequest))]
        public async Task Run(Context context, MessageRequest message)
        {

        }
    }
}
