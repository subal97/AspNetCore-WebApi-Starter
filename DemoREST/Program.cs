using DemoREST.Contracts;
using DemoREST.Data;
using DemoREST.InitialConfiguration;
using DemoREST.Installers;
using DemoREST.Options;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

//Install all services in one shot. CLEAN
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

//Migration on startup
using (var scope = app.Services.CreateScope())
{
    await Seed.RunMigration(scope);
    await Seed.AddRoles(scope);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) => 
    { 
        context.Response.ContentType= "application/json";
        var response = new HealthCheckResponse
        {
            OverallStatus = report.Status.ToString(),
            Checks = report.Entries.Select(e => new HealthCheck
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description
            }),
            Duration = report.TotalDuration
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
});

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

var swaggerOptions = new SwaggerOptions();
builder.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

app.UseSwagger(options =>
{
    options.RouteTemplate = swaggerOptions.JsonRoute;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
});

app.UseMvc();

app.Run();
