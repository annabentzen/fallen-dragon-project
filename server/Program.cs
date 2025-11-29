using DragonGame.Data;
using DragonGame.Repositories;
using DragonGame.Services;
using Microsoft.EntityFrameworkCore;
using server.Services;
using server.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

/*
Explanation of code:
- Single DbContext (AppDbContext) with one SQLite file (DragonGame.db) — avoids confusion.
- Migration first, then seeding — ensures tables exist before you insert data.
- JSON serializer uses camelCase, matching your frontend TypeScript interfaces.
- ReferenceHandler set to IgnoreCycles to prevent $id/$values serialization.
- SEEDING NOW HAPPENS **BEFORE** THE APP ACCEPTS ANY REQUESTS → fixes "No act found" race condition!
*/

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Services ----------------------

// Add controllers with views
builder.Services.AddControllersWithViews();

// Configure JSON serialization (camelCase + no circular refs)
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



// JWT Authentication 
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

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
builder.Services.AddScoped<IAuthService, AuthService>();

// ---------------------- JWT Authentication ----------------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ---------------------- CORS ----------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowReactDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // React/Vite dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ---------------------- Build app ----------------------
var app = builder.Build();

/*
   CRITICAL FIX: Run migrations + seed the database **BEFORE** the app starts listening.
   This prevents the race condition where a player creates a session before the story data exists.
   After this change, "No act found for session X" disappears forever.
*/
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. Apply any pending migrations
    context.Database.Migrate();

    // 2. Seed the full story (The Fallen Dragon) — guaranteed to finish before first request
    await DbSeeder.Seed(context);

    Console.WriteLine("Database migrated and fully seeded. Ready for players!");
}

// ---------------------- Middleware ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Apply CORS policies
app.UseCors("AllowReactDev");   // Specific policy for your React frontend
app.UseCors();                  // Fallback default policy (optional)

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

// ---------------------- Default route ----------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Create}/{id?}");

app.Run();