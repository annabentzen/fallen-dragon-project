using DragonGame.Data;
using DragonGame.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


/*
Explanation of code:
- Single DbContext (AppDbContext) and one SQLite file (Story.db) — avoids confusion.
- Migration first, then seeding — ensures tables exist before you insert data.
- JSON serializer uses camelCase, matching your frontend TypeScript interfaces.
- CORS is configured before middleware, allowing your React frontend on 5173 to call the API.
*/


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure Entity Framework with SQLite (single database)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/DragonGame.db")); // <-- database file in root

// Register repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Apply migrations
    context.Database.Migrate();

    // Seed initial data
    await DbSeeder.Seed(context);
}





// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors(); // use default CORS policy
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route: open character create view first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

app.Run();
