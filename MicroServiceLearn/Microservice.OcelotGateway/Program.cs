using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("ocelotconsul.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(); //ע��
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOcelot();//ֱ���滻�˹ܵ�ģ��


app.Run();
