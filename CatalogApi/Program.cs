using Amazon.DynamoDBv2;
using CatalogApi.Config;
using CatalogApi.Middlewares;
using CatalogApi.Service;
using CatalogApi.Service.DynamoLogging;
using Core.Models;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Log.Logger = new LoggerConfiguration()
//     .Enrich.FromLogContext()
//     .Enrich.WithNewRelicLogsInContext() // método do pacote
//     .WriteTo.File(
//         path: "logs/app.log.json",
//         formatter: new NewRelicFormatter(),
//         rollingInterval: RollingInterval.Day)
//     .CreateLogger();
//
// builder.Host.UseSerilog();


builder.Services.AddDynamoDb(builder.Configuration);

var logTableName    = builder.Configuration["DynamoDb:LogTableName"];

builder.Logging
    .ClearProviders()                      
    .AddConsole()                          
    .AddDynamoDbLogger(logTableName, LogLevel.Information);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddTransient<ICorrelationIdService, CorrelationIdService>();

builder.Services.AddScoped(typeof(IBaseLogger<>), typeof(BaseLogger<>));

// Configuração do cache
builder.Services.AddMemoryCache();
builder.Services.AddTransient<ICacheService, MemCacheService>();

// Registrar repositórios
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlayerLibraryGames,PlayerLibraryGamesRepository>();

// Configuração do HttpClient para comunicação com UserAPI
builder.Services.AddHttpClient("UsersApi", client =>
{
    var usersApiUrl = builder.Configuration["Services:UsersApi:BaseUrl"] ?? "http://users-api:8080/";
    client.BaseAddress = new Uri(usersApiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Registrar serviços de validação de token
builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();

builder.Services.AddHealthChecks();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IRabbitMqService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
    var logger = sp.GetRequiredService<ILogger<RabbitMqService>>();
    
    return RabbitMqService.CreateAsync(settings, logger).GetAwaiter().GetResult();
});

builder.Services.AddHostedService<PaymentProcessConsumer>();


var app = builder.Build();

app.UseLogMiddleware();
app.UseDynamoLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapHealthChecks("/health");

app.UseHttpsRedirection();

// Adicionar middleware de validação JWT customizado
//app.UseMiddleware<JwtValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Catalog API is up");

app.Run();