using System.Globalization;
using Microsoft.AspNetCore.Diagnostics;
using WorkdayCalendar.Application.Services;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;
using WorkdayCalendar.Infrastructure;
using WorkdayCalendar.Ui.Components;
using WorkdayCalendar.Ui.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWorkdayCalendarService, WorkdayCalendarService>();
builder.Services.AddJsonCalendarInfrastructure();

var app = builder.Build();

const string DateFormat = "dd-MM-yyyy";
const string RecurringDateFormat = "dd-MM";
const string DateTimeFormat = "dd-MM-yyyy HH:mm";
const string TimeFormat = "HH:mm";

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var statusCode = exception is ArgumentException or FormatException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;
        await Results.Json(new { error = exception?.Message ?? "Unexpected error." }).ExecuteAsync(context);
    });
});

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSwagger();
app.UseSwaggerUI();

var calendarApi = app.MapGroup("/api/workday-calendar").WithTags("Workday Calendar");

calendarApi.MapGet("/", async (
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    var calendar = await calendarService.GetWorkdayCalendarAsync(cancellationToken);
    return Results.Ok(new WorkdayCalendarApiModel(
        new WorkdayScheduleApiModel(
            calendar.WorkdaySchedule.StartsAt,
            calendar.WorkdaySchedule.StopsAt),
        calendar.Holidays,
        [.. calendar.RecurringHolidays.Select(holiday => new RecurringHolidayApiModel(holiday.Day, holiday.Month))]));
})
    .WithSummary("Get workday calendar configuration")
    .WithDescription("Returns workday hours, fixed holidays, and recurring holidays.");

calendarApi.MapPut("/workday-schedule", async (
    string startsAt,
    string stopsAt,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.SetWorkdayScheduleAsync(
        new WorkdaySchedule(ParseTime(startsAt), ParseTime(stopsAt)),
        cancellationToken);
    return Results.Ok();
})
    .WithSummary("Set workday hours")
    .WithDescription("Sets the workday start and stop times. Parameters: startsAt and stopsAt in HH:mm format, for example startsAt=08:00&stopsAt=16:00.");

calendarApi.MapPost("/holidays", async (
    string date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.AddHolidayAsync(ParseDate(date), cancellationToken);
    return Results.Ok();
})
    .WithSummary("Add fixed holiday")
    .WithDescription("Adds a fixed holiday. Parameter: date in dd-MM-yyyy format, for example 27-05-2004.");

calendarApi.MapDelete("/holidays/{date}", async (
    string date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.RemoveHolidayAsync(ParseDate(date), cancellationToken);
    return Results.Ok();
})
    .WithSummary("Remove fixed holiday")
    .WithDescription("Removes a fixed holiday. Parameter: date in dd-MM-yyyy format, for example 27-05-2004.");

calendarApi.MapPost("/recurring-holidays", async (
    string date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.AddRecurringHolidayAsync(ParseRecurringHoliday(date), cancellationToken);
    return Results.Ok();
})
    .WithSummary("Add recurring holiday")
    .WithDescription("Adds a recurring yearly holiday. Parameter: date in dd-MM format, for example 17-05.");

calendarApi.MapDelete("/recurring-holidays/{date}", async (
    string date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.RemoveRecurringHolidayAsync(ParseRecurringHoliday(date), cancellationToken);
    return Results.Ok();
})
    .WithSummary("Remove recurring holiday")
    .WithDescription("Removes a recurring yearly holiday. Parameter: date in dd-MM format, for example 17-05.");

calendarApi.MapPost("/calculate", async (
    string startDateTime,
    double workdaysToAdd,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    var result = await calendarService.CalculateWorkdaysAsync(
        ParseDateTime(startDateTime),
        workdaysToAdd,
        cancellationToken);

    return Results.Ok(result.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
})
    .WithSummary("Calculate workday datetime")
    .WithDescription("Calculates the result datetime. Parameters: startDateTime in dd-MM-yyyy HH:mm format and workdaysToAdd as a number. Negative values move backward. Example: startDateTime=24-05-2004%2018:05&workdaysToAdd=-5.5.");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();

static DateOnly ParseDate(string value)
{
    return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
}

static DateTime ParseDateTime(string value)
{
    return DateTime.ParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture);
}

static TimeOnly ParseTime(string value)
{
    return TimeOnly.ParseExact(value, TimeFormat, CultureInfo.InvariantCulture);
}

static RecurringHoliday ParseRecurringHoliday(string value)
{
    var parsed = DateOnly.ParseExact(value, RecurringDateFormat, CultureInfo.InvariantCulture);
    return new RecurringHoliday(parsed.Day, parsed.Month);
}
