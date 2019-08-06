using System;
using System.Threading.Tasks;
using Contracts;
using Hetacode.Microless.Attributes;
using Hetacode.Microless.Core;

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
