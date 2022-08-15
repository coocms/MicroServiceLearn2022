using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("configuration.json", false, true);
builder.Services.AddLogging(builder=>
{
    //builder.AddFilter("System", LogLevel.Warning);//¹ýÂËµôÃüÃû¿Õ¼ä
    //builder.AddFilter("Microsoft", LogLevel.Warning);
    //builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
    //builder.AddFilter("Ocelot.Logging.OcelotDiagnosticListener", LogLevel.Warning);
    //builder.AddFilter("Ocelot.Authorisation.Middleware.AuthorisationMiddleware", LogLevel.Warning);
    //builder.AddFilter("Ocelot.Configuration", LogLevel.Warning);
    builder.AddFilter("Ocelot", LogLevel.Warning);
    builder.AddLog4Net();
});
builder.Services.AddOcelot();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseOcelot();
//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
