using AbdulBackend.Middleware;
using AbdulBackend.Services;

// Load environment variables from .env file FIRST
var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFilePath))
{
    DotNetEnv.Env.Load(envFilePath);
}

// Set environment from .env before creating builder
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

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
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Register HttpClient for OpenRouter (30s timeout)
builder.Services.AddHttpClient("OpenRouter", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
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
builder.Services.AddScoped<IBedrockService, BedrockService>();
builder.Services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();

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

// Configure Kestrel to listen on port 5001 for development, 5000 for production
builder.WebHost.ConfigureKestrel(options =>
{
    var port = builder.Environment.IsDevelopment() ? 5001 : 5000;
    options.ListenAnyIP(port);
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