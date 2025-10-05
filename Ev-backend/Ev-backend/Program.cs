using Ev_backend.Config;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Ev_backend.Utils;
using Ev_backend.Repositories;
using Ev_backend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================== CORS ==================
// Allow frontend on localhost:5173 (Vite / React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ================== Controllers ==================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ================== Dependency Injection ==================

// Auth
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();

// User
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

// Station
builder.Services.AddScoped<StationRepository>();
builder.Services.AddScoped<StationService>();

// Booking
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// EV Owner
builder.Services.AddScoped<IEVOwnerRepository, EVOwnerRepository>();
builder.Services.AddScoped<IEVOwnerService, EVOwnerService>();

// Mobile Authentication (only StationOperator & EvOwner login)
builder.Services.AddScoped<MobileAuth>();

// Utils
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

// ================== Swagger ==================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================== MongoDB Config ==================
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// ================== Build App ==================
var app = builder.Build();

// ✅ Test MongoDB connection on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    try
    {
        var command = new BsonDocument("ping", 1);
        await db.RunCommandAsync<BsonDocument>(command);
        Console.WriteLine("✅ MongoDB connected successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
    }
}

// ================== Middleware ==================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS for frontend
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
