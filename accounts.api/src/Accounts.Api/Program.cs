using System.Text.Json;
using System.Text.Json.Serialization;
using Accounts.Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Messaging.Abstractions;
using Messaging.Contracts.Events;
using Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddConfigurationRoot();
var settings = builder.GetSettings();

builder.AddDependence();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader("api-version");
});

builder.Services.AddControllers().AddJsonOptions(o =>
    {
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounts.Api", Version = "v1" });
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

builder.AddDataBase(settings);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = settings.RedisConnection;
    options.InstanceName = settings.RedisInstanceName;
});

builder.Services.AddRabbitMqEventBus(
    builder.Configuration,
    typeof(User_Created_Event).Assembly);

var app = builder.Build();

app.AddMigration();
app.SeedData(settings);


// Configure the HTTP request pipeline.
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
