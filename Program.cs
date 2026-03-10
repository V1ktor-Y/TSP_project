using Microsoft.EntityFrameworkCore;
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
