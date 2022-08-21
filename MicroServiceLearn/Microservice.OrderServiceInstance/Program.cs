using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microservice.Framework;
using Microservice.Framework.HttpApiExtend;
using Microservice.Framework.PollyExtend;
using Microservice.Framework.Register;
using Microservice.OrderServiceInstance.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpInvoker(options =>
{
    options.Message = "This is Program's Message";
});

#region 使用Autofac
{
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>((context, buider) =>
    {
        // 必须使用单例注册
        buider.RegisterType<UserService>()
        .As<IUserService>().SingleInstance().EnableInterfaceInterceptors();

        buider.RegisterType<OrderService>().As<IOrderService>();

        buider.RegisterType<PollyPolicyAttribute>();

    });
}
#endregion 


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOptions"));
builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOptions"));
builder.Services.AddTransient<IConsulRegister, ConsulRegister>();

var app = builder.Build();

#region Consul 注册
app.UseHealthCheckMiddleware("/Api/Health/Index");//心跳请求响应
app.Services.GetService<IConsulRegister>()!.UseConsulRegist();
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
#region API提供区
app.MapPost("/api/order", async (IOrderService orderService, Order order) => {
    var result = await orderService.AddOrder(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "新增成功" : "新增失败",
        StatusCode = result ? 30000 : -9999
    };
});

app.MapPost("/api/aop/order", (IOrderService orderService, Order order) => {
    var result = orderService.AddOrderForAOP(order);
    return new AjaxResult
    {
        Result = result ? true : false,
        Message = result ? "新增成功" : "新增失败",
        StatusCode = result ? 30000 : -9999
    };
});
#endregion
app.MapControllers();

app.Run();
