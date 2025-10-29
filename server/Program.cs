using DragonGame.Data;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<DragonGameDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/DragonGame.db"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=dragongame.db"));

// Register repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();

// Configure CORS BEFORE building the app
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use CORS
app.UseCors(); // default policy

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route: open character create view first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Apply migrations
    context.Database.Migrate();

    // Seed data
    DbSeeder.Seed(context);
}

app.Run();
