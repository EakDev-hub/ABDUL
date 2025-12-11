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

// Register HttpClient for team API (5s timeout)
builder.Services.AddHttpClient("TeamApi", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register HttpClient for OpenRouter (30s timeout)
builder.Services.AddHttpClient("OpenRouter", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
    client.DefaultRequestHeaders.Add("X-Title", "ABDUL Hackathon");
});

// Register custom services
builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();
builder.Services.AddScoped<IHackathonService, HackathonService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IQnaService, QnaService>();
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<ITeamApiClient, TeamApiClient>();
builder.Services.AddScoped<IOpenRouterClient, OpenRouterClient>();

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