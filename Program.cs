using System.Text;
using autenticacion.Database;
using autenticacion.Services;
using Microsoft.EntityFrameworkCore;
using autenticacion.Models;
using Microsoft.AspNetCore.Routing;
using autenticacion.Interface;
using autenticacion.WebSockets;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5201");

var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("MYSQL_CONNECTION_STRING environment variable and DefaultConnection in appsettings.json are both missing.");
}

builder.Services.AddDbContext<DBContext>(options =>
    options.UseMySQL(connectionString));


builder.Services.AddIdentityInfrastruture(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IJwtHandle, JwtHandle>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
    });
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiSwaggerExtension();

var app = builder.Build();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.MapHub<WorkerHub>("/workerhub");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();