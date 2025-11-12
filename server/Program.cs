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
- CORS is configured before middleware, allowing your React frontend (5173) to call the API.
*/

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Services ----------------------

// Add controllers with views
builder.Services.AddControllersWithViews();

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use camelCase in JSON, matching TypeScript frontend
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Ignore object reference cycles instead of Preserve
        // This ensures lists serialize as plain arrays
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Optional: MaxDepth for safety
        options.JsonSerializerOptions.MaxDepth = 64;

        // Optional: pretty-print JSON (for debugging)
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Entity Framework with SQLite (single database)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=App_Data/DragonGame.db")); // database file in root

// Register repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();
builder.Services.AddScoped<IPlayerSessionRepository, PlayerSessionRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterPoseRepository, CharacterPoseRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IPlayerSessionService, PlayerSessionService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IPoseService, PoseService>();
builder.Services.AddScoped<IChoiceHistoryService, ChoiceHistoryService>();



// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddCors(options =>
{
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
app.UseHttpsRedirection();     // Redirect HTTP → HTTPS
app.UseStaticFiles();          // Serve wwwroot files
app.UseRouting();              // Route requests
app.UseAuthorization();        // Authorization middleware

// ---------------------- Default route ----------------------

// Open Character Create view by default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

app.Run();
