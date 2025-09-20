using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TeamTime.Application.Services;
using TeamTime.Domain.Interfaces;
using TeamTime.Infrastructure.Data;
using TeamTime.Infrastructure.Repositories;
using TeamTime.Infrastructure.Services;
using TeamTime.Application.Mappings;
using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;
using TeamTime.Application.Common;
using TeamTime.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using TeamTime.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TeamTime.Infrastructure")));

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Application Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

// Mappers
builder.Services.AddScoped<IMapper<User, UserDto>, UserMapper>();
builder.Services.AddScoped<IMapper<Area, AreaDto>, AreaMapper>();
builder.Services.AddScoped<IMapper<Project, ProjectDto>, ProjectMapper>();
builder.Services.AddScoped<IMapper<TeamTimeTask, TaskDto>, TaskMapper>();
builder.Services.AddScoped<IMapper<TimeEntry, TimeEntryDto>, TimeEntryMapper>();
builder.Services.AddScoped<IMapper<TimePeriod, TimePeriodDto>, TimePeriodMapper>();

// Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Identity Service
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Time Period Service
builder.Services.AddScoped<ITimePeriodService, TimePeriodService>();

// Time Entry Validation Service
builder.Services.AddScoped<ITimeEntryValidationService, TimeEntryValidationService>();

// Auto-register all command handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(ICommandHandler<,>))
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Auto-register all query handlers
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(IQueryHandler<,>))
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Auto-register all validators
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(TeamTime.Application.Validators.IValidator<>))
    .AddClasses(classes => classes.AssignableTo(typeof(TeamTime.Application.Validators.IValidator<>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Authentication with JWT (overriding Identity default)
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!TeamTime2024";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TeamTime";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TeamTime";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => new
{
    Status = "OK",
    Timestamp = DateTime.UtcNow,
    Service = "TeamTime API",
    Version = "1.0.0"
});

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await SeedData.SeedAsync(context, userManager, roleManager);

        // Seed development data only in development environment
        if (app.Environment.IsDevelopment())
        {
            await SeedData.SeedDevelopmentDataAsync(context, userManager);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.Run();