using Ev_backend.Config;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Ev_backend.Utils;
using Ev_backend.Repositories;
using Ev_backend.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================== CORS ==================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ================== Controllers ==================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ✅ allow "EvOwner", "evOwner", "Backoffice", etc.
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        );
        // ✅ make property names case-insensitive
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // ✅ optional: pretty print in Swagger
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ================== Dependency Injection ==================
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<StationRepository>();
builder.Services.AddScoped<StationService>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IEVOwnerRepository, EVOwnerRepository>();
builder.Services.AddScoped<IEVOwnerService, EVOwnerService>();

builder.Services.AddScoped<FinancialRepository>();
builder.Services.AddScoped<FinancialService>();

builder.Services.AddScoped<MobileAuth>();

builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================== MongoDB ==================
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

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

// ✅ MongoDB connection test
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

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();
app.Run();
