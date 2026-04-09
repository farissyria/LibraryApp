using Library.API; // Make sure this matches your project's namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add support for API controllers
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
// 👇 This replaces builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 👇 This replaces app.MapOpenApi();
    app.UseSwagger();      // Generates the OpenAPI specification
    app.UseSwaggerUI();    // Serves the interactive Swagger UI at /swagger
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Map controller routes

// Your weather forecast endpoint can remain, but it's better to move it to a Controller.
// For simplicity, keeping it here is fine for now.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

// You need this record definition if it's not in another file
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}