using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("ocelotconsul.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(); //注入
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOcelot();//直接替换了管道模型


app.Run();
