using Microsoft.EntityFrameworkCore;
using IoTAssesment.Models;
using IoTAssesment.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add custom services using extension methods
builder.Services.AddIoTDeviceManagementServices(builder.Configuration);

// Add CORS configuration for frontend
builder.Services.AddCorsConfiguration();

// Add controllers with views for MVC
builder.Services.AddControllersWithViews();

// Add logging configuration
builder.Services.AddLoggingConfiguration(builder.Configuration);

var app = builder.Build();

// Ensure database is created and migrations are applied
await EnsureDatabaseCreated(app);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable CORS
app.UseCors("AllowVueApp");

app.UseAuthorization();

// Map API routes
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}");

// Map default MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map health checks
app.MapHealthChecks("/health");

app.Run();

/// <summary>
/// Ensures database is created and migrations are applied
/// </summary>
async Task EnsureDatabaseCreated(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<IoTDeviceContext>();

        logger.LogInformation("Checking database and applying migrations...");

        // Ensure database is created and apply any pending migrations
        await context.Database.MigrateAsync();

        logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating/migrating the database");
        throw;
    }
}
