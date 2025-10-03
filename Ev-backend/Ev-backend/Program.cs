using Ev_backend.Config;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Ev_backend.Utils;
using Ev_backend.Repositories;
using Ev_backend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add controllers + enum serializer
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register services & repositories
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<StationRepository>();
builder.Services.AddScoped<StationService>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();   
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IEVOwnerRepository, EVOwnerRepository>(); 
builder.Services.AddScoped<IEVOwnerService, EVOwnerService>();

builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

var app = builder.Build();

// ✅ Test MongoDB connection on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    try
    {
        db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
        Console.WriteLine("✅ MongoDB connected successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
    }
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
