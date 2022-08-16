using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.Responses;
using Ocelot.Values;

namespace Microservice.OcelotGateway.OcelotExtend.LoadBanlanceExtend
{

    public class CustomPollingLoadBalancer : ILoadBalancer
    {
        private readonly Func<Task<List<Service>>> _DownstreamServicesTaskList;
        private readonly object CustomPollingLoadBalancer_Lock = new object();
        private int _lastIndex;

        public CustomPollingLoadBalancer(Func<Task<List<Service>>> services)
        {
            this._DownstreamServicesTaskList = services;
        }

        public async Task<Response<ServiceHostAndPort>> Lease(HttpContext httpContext)
        {
            var downstreamServices = await this._DownstreamServicesTaskList();
            lock (CustomPollingLoadBalancer_Lock)
            {
                Console.WriteLine($"This is {nameof(CustomPollingLoadBalancer)}.Lease");
                Console.WriteLine($"This is {httpContext.Request.Host.Value}");
                Console.WriteLine($"This is {string.Join(",", downstreamServices.Select(s => s.HostAndPort.DownstreamHost + ": " + s.HostAndPort.DownstreamPort))}");
                if (_lastIndex >= downstreamServices.Count)
                {
                    _lastIndex = 0;
                }

                var next = downstreamServices[_lastIndex];
                Console.WriteLine("Request To " + next.HostAndPort.DownstreamPort.ToString());
                _lastIndex++;
                return new OkResponse<ServiceHostAndPort>(next.HostAndPort);
            }
        }

        public void Release(ServiceHostAndPort hostAndPort)
        {
        }
    }
}
