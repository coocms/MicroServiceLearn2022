using Consul;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Invoker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller

    {
        IConfiguration _configuration;
        public TestController(IConfiguration configuration)
        {
            

            _configuration = configuration;
        }
        static int count = 0;
        [HttpGet]
        public IActionResult Index()
        {
            string url = "http://UserMinimalAPIService/api/users/all";
            var l = _configuration.GetValue(typeof(string), "");
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://81.68.119.59:8500/");
                c.Datacenter = "dc1";
            });//找到consul--像DNS

            Dictionary<string, AgentService>? response = client.Agent.Services().Result.Response;//获取Consul全部服务清单
            Uri uri = new Uri(url);
            string groupName = uri.Host;
            AgentService agentService = null;
            var dictionary = response.Where(s => s.Value.Service.Equals(groupName, StringComparison.OrdinalIgnoreCase)).ToArray();
            agentService = dictionary[count++].Value;
            if(count >= response.Count)
                count = 0;
            

            url = $"{uri.Scheme}://{agentService.Address}:{agentService.Port}{uri.PathAndQuery}";

            
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(message).Result;
                string content = result.Content.ReadAsStringAsync().Result;
                return Ok(content);
            }
            
        }
    }
}
