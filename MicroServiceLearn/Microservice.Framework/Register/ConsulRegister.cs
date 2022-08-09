using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.Register
{
    public class ConsulRegister : IConsulRegister
    {
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        private readonly ConsulClientOptions _consulClientOptions;
        private readonly IConfiguration _configuration;
        public ConsulRegister(IOptionsMonitor<ConsulRegisterOptions> consulRegisterOptions, IOptionsMonitor<ConsulClientOptions> consulClientOptions, IConfiguration configuration)
        {
            this._consulRegisterOptions = consulRegisterOptions.CurrentValue;
            this._consulClientOptions = consulClientOptions.CurrentValue;
            _configuration = configuration;
            
        }
        public async Task UseConsulRegist()
        {
            string localIp = GetLocalIp();
            string clientIp = _configuration.GetSection("ConsulRegisterOptions")["IP"];
            string urls = _configuration["urls"];
            if (clientIp == "0.0.0.0")
            {
                _consulRegisterOptions.IP = localIp;
                _consulRegisterOptions.Port = int.Parse(urls.Split(':').LastOrDefault().ToString());
                var uri = new Uri(_consulRegisterOptions.HealthCheckUrl.ToString());
                _consulRegisterOptions.HealthCheckUrl = string.Format("http://{0}:{1}{2}", _consulRegisterOptions.IP, _consulRegisterOptions.Port, uri.AbsolutePath);
                
            }
            
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri($"http://{this._consulClientOptions.IP}:{this._consulClientOptions.Port}/");
                c.Datacenter = this._consulClientOptions.Datacenter;
            }))

            {
                await client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    
                    ID = $"{this._consulRegisterOptions.GroupName}-{this._consulRegisterOptions.IP}-{this._consulRegisterOptions.Port}",//唯一Id
                    Name = this._consulRegisterOptions.GroupName,//组名称-Group
                    Address = this._consulRegisterOptions.IP,
                    Port = this._consulRegisterOptions.Port,
                    Tags = new string[] { this._consulRegisterOptions.Tag ?? "Tags is null" },
                    Check = new AgentServiceCheck()
                    {
                        Interval = TimeSpan.FromSeconds(this._consulRegisterOptions.Interval),
                        HTTP = this._consulRegisterOptions.HealthCheckUrl,
                        Timeout = TimeSpan.FromSeconds(this._consulRegisterOptions.Timeout),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(this._consulRegisterOptions.DeregisterCriticalServiceAfter),
                    }
                });
                Console.WriteLine($"{JsonConvert.SerializeObject(this._consulRegisterOptions)} 完成注册");
            }
        }
        public string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                    return AddressIP;
                }
            }
            return AddressIP;
        }
    }
}
