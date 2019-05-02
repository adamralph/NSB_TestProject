using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace MultipleEndPointNSB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            EndPointReAuth endPointReAuth = new EndPointReAuth();
            EndPointPfsServices endPointPfsServices = new EndPointPfsServices();

            await endPointPfsServices.Start();
            await endPointReAuth.Start();

            var senderConfig = new EndpointConfiguration("pfs-sender");
            senderConfig.SendOnly();

            var routing = senderConfig.UseTransport<MsmqTransport>().Routing();
            routing.RouteToEndpoint(typeof(ReloadServicesCommand), "pfs.services");

            senderConfig.UsePersistence<NHibernatePersistence>();
            senderConfig.UseSerialization<JsonSerializer>();
            senderConfig.EnableInstallers();

            var sender = await Endpoint.Start(senderConfig).ConfigureAwait(false);
            await sender.Send(new ReloadServicesCommand { ServiceIds = new[] { 1, 2, 3 } }).ConfigureAwait(false);

            Console.ReadLine();

            await sender.Stop();
            await endPointReAuth.Stop();
            await endPointPfsServices.Stop();
        }
    }
}
