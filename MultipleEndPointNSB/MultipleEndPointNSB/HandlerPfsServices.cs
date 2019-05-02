using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleEndPointNSB
{
    class HandlerPfsServices : IHandleMessages<ReloadServicesCommand>
    {
        public async Task Handle(ReloadServicesCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"ReloadServiceCommandRecieved {string.Concat(message.ServiceIds.Select(id => id.ToString() + " "))}");
            foreach (var serviceId in message.ServiceIds)
            {
                await context.Publish(new ReAuthorizationEvent { ServiceId = serviceId, ContractId = serviceId * 10, SubscriberId = serviceId * 100 });
            }
        }
    }
}
