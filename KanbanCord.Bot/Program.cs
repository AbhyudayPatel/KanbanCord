using KanbanCord.Bot.Extensions;

namespace KanbanCord.Bot;

internal class Program
{
    private static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();
        
        // Configure URL binding for deployment
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
        
        // Ensure production environment is set for containerized deployments
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
        {
            builder.Environment.EnvironmentName = "Production";
        }
        
        builder.Host
            .UseDefaultServiceProvider()
            .UseSerilog();
        
        builder.Services
            .AddHostDependencies()
            .AddDiscordConfiguration(builder.Configuration);

        builder.Services.AddHealthChecks()
            .AddKanbanCordHealthChecks();
        
        var app = builder.Build();

        app.UseHealthChecks("/health");
        
        // Log the application configuration
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Application starting on port {Port}", port);
        logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
        
        await app.RunAsync();
    }
}