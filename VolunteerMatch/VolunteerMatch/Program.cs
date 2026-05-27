using Serilog;
using Serilog.Events;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Ai;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Infrastructure.Helpers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: "C:/Temp/VolunteerMatchLogs/log-.txt",
        rollingInterval: RollingInterval.Day,
        encoding: Encoding.UTF8,
        shared: true)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VolunteerMatch API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "ჩაწერე ტოკენი ასე: your token value without \"\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<VolunteerMatchingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<MyOrganizationService>();
builder.Services.AddScoped<MyVolunteerService>();
builder.Services.AddScoped<VolunteerService>();
builder.Services.AddScoped<MyOrganizationEventsService>();
builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<OrganizationEventsService>();
builder.Services.AddScoped<IVolunteerTagService, VolunteerTagService>();
builder.Services.AddScoped<ITagValidator, TagValidator>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<IEventTagService, EventTagService>();
builder.Services.AddScoped<FavoriteEventService>();
//builder.Services.AddScoped<INotificationFactory, NotificationFactory>();
//builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAiMatchingClient, OpenAiMatchingClient>();
builder.Services.AddScoped<IVolunteerMatchingService, MyVolunteerMatchingService>();
builder.Services.AddScoped<IAiMatchingLogger, AiMatchingLogger>();
builder.Services.AddScoped<MatchingQueryHelper>();
builder.Services.AddScoped<FavoritesHelper>();
builder.Services.AddScoped<MatchSaveHelper>();
builder.Services.AddScoped<MatchCleanupHelper>();
builder.Services.AddScoped<IOrganizationMatchingService, MyOrganizationMatchingService>();


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

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)
        )
    };
});
builder.Services.AddAuthorization(options => {
    // policy for organization
    options.AddPolicy("OrganizationPolicy",
        policy => policy.RequireClaim(ClaimTypes.Role, "ორგანიზაცია"));

    // policy for volunteer
    options.AddPolicy("VolunteerPolicy",
        policy => policy.RequireClaim(ClaimTypes.Role, "მოხალისე"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
