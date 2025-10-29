using DragonGame.Data;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<DragonGameDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/DragonGame.db"));

// Add Story database (also SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/Story.db"));

// Register repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:5173")
           .AllowAnyHeader()
           .AllowAnyMethod());


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default route: open character create view first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

app.Run();
