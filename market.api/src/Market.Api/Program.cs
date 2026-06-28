using Market.Application.Interfaces;
using Market.Application.Services;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;
using Market.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Market.Api.Configurations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Messaging.Contracts.Events;
using Messaging.RabbitMQ;
using Market.Infrastructure.Repositories.Externals;
using Market.Domain.Interfaces.Externals;
using Market.Api.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.AddConfigurationRoot();
var settings = builder.GetSettings();

builder.Configuration.AddEnvironmentVariables();

var connectionString = settings.ConnectionString;

builder.Services.AddScoped<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductStockRepository, ProductStockRepository>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
builder.Services.AddScoped<IProductPhotoRepository, ProductPhotoRepository>();
builder.Services.AddScoped<IProductInformationRepository, ProductInformationRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
builder.Services.AddScoped<IClientAuthorizationRepository, ClientAuthorizationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductStockService, ProductStockService>();
builder.Services.AddScoped<IProductPriceService, ProductPriceService>();
builder.Services.AddScoped<IProductPhotoService, ProductPhotoService>();
builder.Services.AddScoped<IProductInformationService, ProductInformationService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();
builder.Services.AddScoped<PokemonProductSeeder>();
builder.Services.AddScoped<DigimonProductSeeder>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = settings.AccountsApiUrl;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Market.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                },
                new List<string>()
            }
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddRabbitMqEventBus(
    builder.Configuration,
    typeof(Project_Created_Event).Assembly);

var app = builder.Build();

var runner = new MigrationRunner(
    connectionString: connectionString,
    migrationsFolder: Path.Combine(AppContext.BaseDirectory, "Migrations")
);

await runner.RunAsync();

using (var scope = app.Services.CreateScope())
{
    var pokemonSeeder = scope.ServiceProvider.GetRequiredService<PokemonProductSeeder>();
    var digimonSeeder = scope.ServiceProvider.GetRequiredService<DigimonProductSeeder>();
    //await pokemonSeeder.SeedAsync();
    //await digimonSeeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            var basePath = httpReq.Headers["X-Forwarded-Prefix"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(basePath) &&
                Uri.TryCreate(httpReq.Headers.Referer.FirstOrDefault(), UriKind.Absolute, out var referer))
            {
                var swaggerPathIndex = referer.AbsolutePath.IndexOf("/swagger", StringComparison.OrdinalIgnoreCase);
                if (swaggerPathIndex > 0)
                    basePath = referer.AbsolutePath[..swaggerPathIndex];
            }

            if (!string.IsNullOrWhiteSpace(basePath))
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new() { Url = basePath.TrimEnd('/') }
                };
            }
        });
    });
    app.UseSwaggerUI();
}

app.UseCors(builder =>
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
