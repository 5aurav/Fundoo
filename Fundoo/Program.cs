using BusinessLayer.Interface;
using BusinessLayer.Notes.Commands;
using BusinessLayer.Notes.Queries;
using BusinessLayer.Service;
using BusinessLayer.User.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middleware.ExceptionHandling;
using Middleware.Services.Cache;
using Middleware.Services.Email;
using Middleware.Services.Hashing;
using Middleware.Services.JWT;
using NLog.Web;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Repository;
using RepositoryLayer.Service;
using StackExchange.Redis;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FundooCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IUserBL, UserBL>();

builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<LoginUserCommandHandler>();
builder.Services.AddScoped<ForgotPasswordCommandHandler>();
builder.Services.AddScoped<ResetPasswordCommandHandler>(); 

builder.Services.AddScoped<IUserRL, UserRL>();

builder.Services.AddScoped<AddNoteCommandHandler>();
builder.Services.AddScoped<EditNoteCommandHandler>();
builder.Services.AddScoped<DeleteNoteCommandHandler>();
builder.Services.AddScoped<ToggleArchiveCommandHandler>();
builder.Services.AddScoped<ToggleTrashCommandHandler>();

builder.Services.AddScoped<GetAllNotesQueryHandler>();
builder.Services.AddScoped<GetNoteByIdQueryHandler>();

builder.Services.AddScoped<INotesBL, NotesBL>();


builder.Services.AddScoped<INotesRL, NotesRL>();

builder.Services.AddScoped<IPasswordHashing, PasswordHashing>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<RegisterUserCommandHandler>();

builder.Services.AddDbContext<FundooContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FundooConnection")
    );
});

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        ),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fundoo API",
        Version = "v1",
        Description = "Fundoo Notes REST API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


var redisConnection =
    builder.Configuration["Redis:ConnectionString"];

if (string.IsNullOrWhiteSpace(redisConnection))
{
    if (builder.Environment.IsProduction())
    {
        throw new InvalidOperationException(
            "Redis:ConnectionString must be configured in production.");
    }

    builder.Services.AddDistributedMemoryCache();
}
else
{
    var redisOptions =
        ConfigurationOptions.Parse(redisConnection);

    redisOptions.AbortOnConnectFail = false;
    redisOptions.ConnectTimeout = 5000;

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConfigurationOptions = redisOptions;
        options.InstanceName = "fundoo:";
    });
}

builder.Services.AddSingleton<ICacheService, RedisCacheService>();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FundooCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();