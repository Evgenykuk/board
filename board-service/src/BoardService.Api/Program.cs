using BoardService.Api.Extensions;
using BoardService.Infrastructure.Data;
using BoardService.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Сервисы
builder.Services.AddControllers();

// Подключение к БД
var connectionString = builder.Configuration.GetConnectionString("BoardDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'BoardDb' is missing.");
}

builder.Services.AddDbContext<BoardDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null))
    .UseSnakeCaseNamingConvention());

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

// Kafka Consumer
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddHostedService<KafkaConsumerService>();

// CORS (для фронтенда)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Board Service API",
        Description = "API for managing aircraft (planes), state, checklist, and boarding."
    });

    // Подключить XML-комментарии, если есть
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Board Service API V1");
        c.RoutePrefix = "swagger"; // Доступ по /swagger
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health")
   .WithTags("Health");

app.MapHealthChecks("/health/db")
   .WithTags("Health");

app.Run();
