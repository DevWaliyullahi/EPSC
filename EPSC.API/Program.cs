using EPSC.Application.Interfaces.IMemberService;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Infrastructure.Configurations.Initializers;
using EPSC.Infrastructure.Identity.Auth;
using EPSC.Services.Handler.Member;
using EPSC.Services.Repositories;
using EPSC.Services.Validations.Member;
using EPSC.Utility.Configurations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration 
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<GlobalSettings>(builder.Configuration.GetSection("GlobalSettings"));

// DbContext
builder.Services.AddDbContext<EPSCDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
        sqlOptions.MigrationsAssembly("EPSC.Infrastructure");
    });
});

// Registering services
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();


// Identity with custom user and role
builder.Services.AddIdentity<EPSAuthUser, EPSAuthRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<EPSCDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
if (jwtSettingsSection.Exists())
{
    var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
    if (jwtSettings != null)
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });
    }
    else
    {
        throw new InvalidOperationException("JwtSettings configuration is missing or invalid.");
    }
}
else
{
    throw new InvalidOperationException("JwtSettings section is missing in the configuration.");
}

// Hangfire setup
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Controller setup with FluentValidation registration
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<MemberCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MemberUpdateDtoValidator>();

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "EPSC API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Scoped services
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EPSC API v1");
        c.RoutePrefix = string.Empty;
    });
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.SeedAsync();
}

app.MapControllers();
app.Run();
