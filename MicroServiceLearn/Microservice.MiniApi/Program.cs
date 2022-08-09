using Microserivce.Interface;
using Microservice.Framework;
using Microservice.Framework.Register;
using Microservice.Model;
using Microservice.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//如果需要获取HttpContext
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IConsulRegister, ConsulRegister>();
builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOptions"));
builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOptions"));

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

#region Consul 注册
app.UseHealthCheckMiddleware("/Api/Health/Index");//心跳请求响应
app.Services.GetService<IConsulRegister>()!.UseConsulRegist();
#endregion

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/api/users/all", (IUserService userService, IHttpContextAccessor httpContextAccessor) =>
{
    Console.WriteLine($"This is UsersController {app.Configuration["port"] ?? app.Configuration["port"]} Invoke");
    var host = httpContextAccessor.HttpContext!.Request.Host;

    return userService.UserAll().Select(u => new User()
    {
        Id = u.Id,
        Account = u.Account + "MA",
        Name = u.Name,
        Role = $"{app.Configuration["ip"] ?? host.Host}" +
        $"{app.Configuration["port"] ?? (host.Port is null ? "NonePort" : host.Port!.Value.ToString())}",
        Email = u.Email,
        LoginTime = u.LoginTime,
        Password = u.Password + "K8S"
    });

});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}