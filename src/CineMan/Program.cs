using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using CineMan.Persistence.Data;
using CineMan.Persistence;
using CineMan.Options;
using CineMan.Services;
using CineMan.Services.Utils;
using Microsoft.OpenApi.Models;
using CineMan.Middlewares;
using CineMan.Extensions;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.AddLogging(configure =>
{
    configure.ClearProviders();
    configure.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.TimestampFormat = "[HH:mm:ss] ";
        options.UseUtcTimestamp = true;
    });
    configure.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", builder =>
    {

        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(AuthOptions.Auth));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.Jwt));

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CineMan",
        Description = "Yet another movie booking API",
        Contact = new OpenApiContact
        {
            Name = "- Email",
            Email = "pjsm3701@gmail.com"
        },
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format 'Bearer {token}'",
        Name = "Authorization"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger()
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CineMan v1");
        });
    app.SeedData();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler();

app.UseCors("AllowHost");
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();