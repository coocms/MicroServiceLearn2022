using Microservice.OcelotGateway.OcelotExtend.LoadBanlanceExtend;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Microservice.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("ocelotconsul.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCustomLoadBalancer((serviceProvider, Route, serviceDiscoveryProvider) => new CustomPollingLoadBalancer(serviceDiscoveryProvider.Get))
    .AddConsul()
    .AddPolly()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();//默认字典存储
    }); //注入

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region jwt校验  HS
JWTTokenOptions tokenOptions = new JWTTokenOptions();
builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
string authenticationProviderKey = "UserGatewayKey";

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Bearer Scheme
.AddJwtBearer(authenticationProviderKey, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //JWT有一些默认的属性，就是给鉴权时就可以筛选了
        ValidateIssuer = true,//是否验证Issuer
        ValidateAudience = true,//是否验证Audience
        ValidateLifetime = true,//是否验证失效时间---默认还添加了300s后才过期
        ClockSkew = TimeSpan.FromSeconds(0),//token过期后立马过期
        ValidateIssuerSigningKey = true,//是否验证SecurityKey

        ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey
    };
});
#endregion




var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot V1");
        c.SwaggerEndpoint("/webapiV1/swagger/v1/swagger.json", "WebAPI V1");
        c.SwaggerEndpoint("/webapiV1/swagger/v1/swagger.json", "WebAPI V2");
    });
}
// Configure the HTTP request pipeline.
app.UseOcelot();//直接替换了管道模型


app.Run();
