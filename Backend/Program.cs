using AbdulBackend.Middleware;
using AbdulBackend.Services;

// Load environment variables from .env file
var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFilePath))
{
    DotNetEnv.Env.Load(envFilePath);
}

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ABDUL Backend API",
        Version = "v1",
        Description = "Backend API for ABDUL project - Managing health check records in PostgreSQL database"
    });
});

// Register custom services
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Kestrel to listen on port 5000
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ABDUL Backend API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();