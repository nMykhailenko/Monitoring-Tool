using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.API.Filters;
using MonitoringTool.Application;
using MonitoringTool.Infrastructure;
using MonitoringTool.Infrastructure.Database;
using Bootstrapper = MonitoringTool.Application.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMvc(options =>
    {
        options.EnableEndpointRouting = false;
        options.Filters.Add<ValidationFilter>();
    })
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(Bootstrapper)));
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

var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();
dbContext.Database.Migrate();

app.Run();