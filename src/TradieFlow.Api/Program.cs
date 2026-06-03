using Microsoft.EntityFrameworkCore;
using TradieFlow.Infrastructure.Data;
using TradieFlow.Application.Interfaces;
using TradieFlow.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddDbContext<TradieFlowDbContext>(options =>
    options.UseNpgsql(BuildConnectionString(builder.Configuration)));

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173")  
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
static string BuildConnectionString(IConfiguration config)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (string.IsNullOrEmpty(databaseUrl))
        return config.GetConnectionString("DefaultConnection")!;

    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var dbPort = uri.Port > 0 ? uri.Port : 5432;   
    return $"Host={uri.Host};Port={dbPort};Database={uri.AbsolutePath.TrimStart('/')};" +
           $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
public partial class Program
{
    
}
