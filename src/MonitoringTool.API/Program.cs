using System.Linq;
using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MonitoringTool.API.Filters;
using MonitoringTool.Application;
using MonitoringTool.Infrastructure;
using MonitoringTool.Infrastructure.Database;
using Swashbuckle.AspNetCore.SwaggerGen;
using Bootstrapper = MonitoringTool.Application.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ValidationFilter));
})
.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining(typeof(Bootstrapper));
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.SwaggerDoc("v1", new()
    {
        Title = "MonitoringTool.API", Version = "v1"
    }); 
    
    c.CustomOperationIds(apiDesc 
        => apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonitoringTool.API v1"));

app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();

var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
dbContext.Database.Migrate();

app.Run();