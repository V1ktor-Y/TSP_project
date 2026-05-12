using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using tsp.Contexts;
using tsp.Repositories;
using tsp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IVFileRepository, VFileRepository>();
builder.Services.AddScoped<IVFileService, VFileService>();
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();
System.Console.WriteLine(builder.Configuration["ConnectionString"]);
builder.Services.AddDbContext<FileDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionString"]));

var jwtKey = builder.Configuration["JwtKey"] ?? "development-only-jwt-key-change-before-production-32";
var jwtIssuer = builder.Configuration["JwtIssuer"] ?? "tsp";
var jwtAudience = builder.Configuration["JwtAudience"] ?? "tsp";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["tsp_auth"];
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (!context.Response.HasStarted && context.Request.Headers.Accept.ToString().Contains("text/html"))
                {
                    context.HandleResponse();
                    context.Response.Redirect("/Account/Login");
                }

                return Task.CompletedTask;
            }
        };
    });
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

if (builder.Configuration["ShouldMigrate"] == "true")
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<FileDbContext>();
        db.Database.Migrate();
    }
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
