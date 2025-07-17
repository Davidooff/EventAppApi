using System.Text;
using Application.Interfaces;
using Application.Services;
using Domain.Options;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//IOptions - data from appsettings.json to pass into DI
builder.Services.Configure<DatabaseOption>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOption>>().Value;
    Console.WriteLine(dbOptions.ConnectionString);
    options.UseNpgsql(dbOptions.ConnectionString);
});

// Add identity manager
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
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
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            // ValidateIssuer = true,
            // ValidateAudience = true,
            // ValidAudience = builder.Configuration["Jwt:Audience"],
            // ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });
//builder.Services.AddAuthorization(); 

builder.Services.AddTransient<ITokenGenerator, TokenService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<UsersExceptionFilter>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UsePathBase(new PathString("/api"));

/*app.UseAuthorization();*/

app.MapControllers();

// app.UseHttpsRedirection();

app.Run();