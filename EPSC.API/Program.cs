using EPSC.Application.Interfaces.IContributionService;
using EPSC.Application.Interfaces.IMemberService;
using EPSC.Infrastructure.Configurations;
using EPSC.Infrastructure.Configurations.Data;
using EPSC.Infrastructure.Configurations.Initializers;
using EPSC.Infrastructure.Hangfire;
using EPSC.Infrastructure.Middleware;
using EPSC.Services.Handler.Contribution;
using EPSC.Services.Handler.Member;
using EPSC.Services.Repositories.Contribution;
using EPSC.Services.Repositories.Member;
using EPSC.Services.Validations.Member;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// DbContext
builder.Services.AddDbContext<EPSCDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
        sqlOptions.MigrationsAssembly("EPSC.Infrastructure");
    });
});

// Authentication & Authorization
builder.Services.InitService(builder.Configuration);

// Repository services
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IContributionRepository, ContributionRepository>();

// Application services
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IContributionService, ContributionService>();

// Database Initializer
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

// Hangfire setup - Background job processing
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Hangfire server
builder.Services.AddHangfireServer();

// Controller setup with FluentValidation registration
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// FluentValidation setup
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Register all validators
builder.Services.AddValidatorsFromAssembly(typeof(MemberCreateDtoValidator).Assembly);


// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "EPSC API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        opt.IncludeXmlComments(xmlPath);
    }

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

// Add Role-based Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireEmployerRole", policy => policy.RequireRole("Employer"));
    options.AddPolicy("RequireContributorRole", policy => policy.RequireRole("Contributor"));
});




var app = builder.Build();

// global error handling middleware 
app.UseMiddleware<GlobalErrorHandlingMiddleware>();


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

// Secure Hangfire dashboard with Admin-only access
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}


// Database seeding with error logging
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.SeedAsync();
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred during database seeding.");
        throw; 
    }
}

// Map controllers
app.MapControllers();

app.Run();