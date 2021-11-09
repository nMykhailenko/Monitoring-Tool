using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.Common.Extensions;
using MonitoringTool.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterModule<CommunicationModule>();
builder.Services.RegisterModule<HealthCheckModule>();
builder.Services.RegisterModule(new DatabaseModule(builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => 
{ 
    c.SwaggerDoc("v1", new()
    {
        Title = "MonitoringTool.API", Version = "v1"
    }); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonitoringTool.API v1"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();