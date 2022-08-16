using Microservice.OcelotGateway.OcelotExtend.LoadBanlanceExtend;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Microservice.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("ocelotconsul.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCustomLoadBalancer((serviceProvider, Route, serviceDiscoveryProvider) => new CustomPollingLoadBalancer(serviceDiscoveryProvider.Get)).AddConsul()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();//Ĭ���ֵ�洢
    }); //ע��

#region jwtУ��  HS
JWTTokenOptions tokenOptions = new JWTTokenOptions();
builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
string authenticationProviderKey = "UserGatewayKey";

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Bearer Scheme
.AddJwtBearer(authenticationProviderKey, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
        ValidateIssuer = true,//�Ƿ���֤Issuer
        ValidateAudience = true,//�Ƿ���֤Audience
        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��---Ĭ�ϻ������300s��Ź���
        ClockSkew = TimeSpan.FromSeconds(0),//token���ں��������
        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey

        ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt������һ��
        ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//�õ�SecurityKey
    };
});
#endregion




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOcelot();//ֱ���滻�˹ܵ�ģ��


app.Run();
