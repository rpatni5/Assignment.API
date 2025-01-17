using Assignment.Contracts.Services;
using Assignment.Data.Contract;
using Assignment.Data.Enitities;
using Assignment.Data.Repositories;
using Assignment.Extensions;
using Assignments.Services.Services;
using Assignments.Utilities.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // For UseInMemoryDatabase

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);



#region configure services
// Add services to the container.
IServiceCollection services = builder.Services;
services.AddControllers();
services.AddCors(builder, MyAllowSpecificOrigins);
services.AddEndpointsApiExplorer();
services.AddSwagger();
services.AddAuthorization();
services.AddHttpContextAccessor();
services.AddDbContext<MovieContext>(options =>
    options.UseInMemoryDatabase("Movie"));
services.AddScoped<IMovieService, MovieService>();
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

services.AddMemoryCache();
services.AddLogging(builder =>
{
    builder.AddFilter($"logs/log_for.txt", LogLevel.Error);
});
services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
    options.HandshakeTimeout = TimeSpan.FromMinutes(2);
});
#endregion
#region configure
var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MovieContext>();
context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{

    app.UseCors(MyAllowSpecificOrigins);
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (!Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
        await next();
    }
    else
    {
        await next();
    }
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Use((ctx, next) => {
    ctx.Response.Headers.Add("Access-Control-Expose-Headers", "*");
    return next();
});
app.Run();
#endregion


