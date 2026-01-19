using CatalogApi.Middlewares;
using CatalogApi.Service;
using Core.Models;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json").Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped);

// Configuração do cache
builder.Services.AddMemoryCache();
builder.Services.AddTransient<ICacheService, MemCacheService>();

// Registrar repositórios
builder.Services.AddScoped<IGameRepository, GameRepository>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adicionar middleware de validação JWT customizado
app.UseMiddleware<JwtValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();