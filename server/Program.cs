using DragonGame.Data;
using DragonGame.Repositories;
using DragonGame.Services;
using Microsoft.EntityFrameworkCore;
using server.Services;
using server.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

/*
Explanation of code:
- Single DbContext (AppDbContext) with one SQLite file (DragonGame.db) — avoids confusion.
- Migration first, then seeding — ensures tables exist before you insert data.
- JSON serializer uses camelCase, matching your frontend TypeScript interfaces.
- ReferenceHandler set to IgnoreCycles to prevent $id/$values serialization.
*/

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Services ----------------------

// Add controllers with views
builder.Services.AddControllersWithViews();

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 64;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/DragonGame.db"));

// ---------------------- Dependency Injection ----------------------

// Register generic repository for all entities
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register specific repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();
builder.Services.AddScoped<IPlayerSessionRepository, PlayerSessionRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IChoiceHistoryRepository, ChoiceHistoryRepository>();

// Register services
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IPlayerSessionService, PlayerSessionService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IPoseService, PoseService>();
builder.Services.AddScoped<IChoiceHistoryService, ChoiceHistoryService>();

// ---------------------- CORS ----------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowLocalhost", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ---------------------- Build app ----------------------
var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Apply pending migrations
    context.Database.Migrate();

    // Seed initial data
    await DbSeeder.Seed(context);
}

// ---------------------- Middleware ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors();                  // Enable CORS globally
app.UseCors("AllowLocalhost");  // Enable specific CORS policy
app.UseHttpsRedirection();     
app.UseStaticFiles();          
app.UseRouting();              
app.UseAuthorization();        

// ---------------------- Default route ----------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

app.Run();
