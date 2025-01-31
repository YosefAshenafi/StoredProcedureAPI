using StoredProcedureAPI.Models;
using StoredProcedureAPI.Repositories;
using StoredProcedureAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Add this for runtime compilation of Razor views

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Stored Procedure API",
        Version = "v1",
        Description = "API for executing stored procedures"
    });
    
    // Add custom operation IDs to distinguish between conflicting actions
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.ActionDescriptor.RouteValues["action"];
    });
});

// Add Authorization services
builder.Services.AddAuthorization();

// Configure database settings
var databaseSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
builder.Services.AddSingleton(databaseSettings);

// Register repository
builder.Services.AddScoped<IStoredProcedureRepository, StoredProcedureRepository>();
builder.Services.AddScoped<IStoredProcedureService, StoredProcedureService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stored Procedure API V1");
        c.InjectStylesheet("/swagger-ui/custom.css");
        c.InjectJavascript("/swagger-ui/custom.js");
    });
}
app.UseStaticFiles(); // Add this to serve static files
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Configure endpoints
app.MapControllers(); // Add this for API controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=StoredProcedure}/{action=Index}/{id?}");

app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



