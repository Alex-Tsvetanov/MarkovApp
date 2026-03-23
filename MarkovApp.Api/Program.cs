using MarkovApp.Configuration;
using MarkovApp.Services;
using MarkovApp.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<CalculationSettings>(
    builder.Configuration.GetSection("CalculationSettings"));

builder.Services.AddScoped<IMarkovCalculatorService, MarkovCalculatorService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
