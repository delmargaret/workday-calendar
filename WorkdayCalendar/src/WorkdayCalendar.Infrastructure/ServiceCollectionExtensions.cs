using Microsoft.Extensions.DependencyInjection;
using WorkdayCalendar.Application.Abstractions;
using WorkdayCalendar.Infrastructure.Json;

namespace WorkdayCalendar.Infrastructure;

/// <summary>
/// Provides infrastructure dependency injection registrations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the JSON-specific workday calendar infrastructure.
    /// </summary>
    /// <param name="services">The service collection to register dependencies in.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddJsonCalendarInfrastructure(this IServiceCollection services)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Json", "calendar-settings.json");
        services.AddSingleton<IWorkdayCalendarRepository>(_ => new JsonWorkdayCalendarRepository(filePath));
        return services;
    }
}
