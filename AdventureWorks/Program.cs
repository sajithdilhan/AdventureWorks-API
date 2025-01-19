using AdventureWorks.Database;
using AdventureWorks.Models.Identity;
using AdventureWorks.Repositories;
using AdventureWorks.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var jwtOptionsSection = builder.Configuration.GetRequiredSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtOptionsSection);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorks"));
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", jwtOptions =>
    {
        var configKey = jwtOptionsSection["Key"];
        var key = Encoding.UTF8.GetBytes(configKey?? string.Empty);

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptionsSection["Issuer"],
            ValidAudience = jwtOptionsSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(IdentityData.AdminUserPolicyName, policy => policy.RequireRole(IdentityData.AdminUserRoleName));
    options.AddPolicy(IdentityData.NormalUserPolicyName, policy => policy.RequireRole(IdentityData.NormalUserRoleName));
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true; // Optional: for better readability
});
builder.Services.AddOpenApi();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Adventure Works", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

// Add custom services
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<IPersonService, MemoryCachedPersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Adventure Works v1");
        options.RoutePrefix = string.Empty; // Set the Swagger UI at the root URL
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();