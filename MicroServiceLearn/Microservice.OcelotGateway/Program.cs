using Microservice.OcelotGateway.OcelotExtend.LoadBanlanceExtend;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("ocelotconsul.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCustomLoadBalancer((serviceProvider, Route, serviceDiscoveryProvider) => new CustomPollingLoadBalancer(serviceDiscoveryProvider.Get)).AddConsul()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();//Ĭ���ֵ�洢
    }); //ע��
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOcelot();//ֱ���滻�˹ܵ�ģ��


app.Run();
