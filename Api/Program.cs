using System.Text;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Options;
using Infrastructure.Redis;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using WebApplication1.Authorization;
using WebApplication1.Authorization.RequirementsData;
using WebApplication1.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//IOptions - data from appsettings.json to pass into DI
builder.Services.Configure<DatabaseOption>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IAuthorizationHandler,
    AdminAuthHandler>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOption>>().Value;
    Console.WriteLine(dbOptions.ConnectionString);
    options.UseNpgsql(dbOptions.ConnectionString);
});

// Add identity manager
builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders(); //token provider for custom strats as forgetPassword, account code verification 

// 3. Configure JWT Authentication
// This tells the application to check for a JWT in the request header
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Try to read the token from the cookie
                context.Token = context.Request.Cookies[builder.Configuration["Jwt:AccessTokenStorage"]];
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters(){
            ValidateIssuer = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = false,
            ValidAudience = builder.Configuration["Jwt:Audience"],
        
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });
//builder.Services.AddAuthorization(); 
builder.Services.AddScoped<RedisUserService>();
builder.Services.AddScoped<RedisSessionsService>();
builder.Services.AddSingleton<ITokenGenerator, TokenService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<UsersExceptionFilter>();

builder.Services.AddControllers();

var app = builder.Build();

var muxer = app.Services.GetRequiredService<IConnectionMultiplexer>();
RedisSessionsService.CreateIndex(muxer);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UsePathBase(new PathString("/api"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.UseHttpsRedirection();

app.Run();