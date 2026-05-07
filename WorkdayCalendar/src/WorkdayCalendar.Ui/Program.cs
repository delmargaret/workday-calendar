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
});

calendarApi.MapPut("/workday-schedule", async (
    TimeOnly startsAt,
    TimeOnly stopsAt,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.SetWorkdayScheduleAsync(new WorkdaySchedule(startsAt, stopsAt), cancellationToken);
    return Results.Ok();
});

calendarApi.MapPost("/holidays", async (
    DateOnly date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.AddHolidayAsync(date, cancellationToken);
    return Results.Ok();
});

calendarApi.MapDelete("/holidays/{date}", async (
    DateOnly date,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.RemoveHolidayAsync(date, cancellationToken);
    return Results.Ok();
});

calendarApi.MapPost("/recurring-holidays", async (
    int day,
    int month,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.AddRecurringHolidayAsync(new RecurringHoliday(day, month), cancellationToken);
    return Results.Ok();
});

calendarApi.MapDelete("/recurring-holidays/{day:int}/{month:int}", async (
    int day,
    int month,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    await calendarService.RemoveRecurringHolidayAsync(new RecurringHoliday(day, month), cancellationToken);
    return Results.Ok();
});

calendarApi.MapPost("/calculate", async (
    DateTime startDateTime,
    double workdaysToAdd,
    IWorkdayCalendarService calendarService,
    CancellationToken cancellationToken) =>
{
    var result = await calendarService.CalculateWorkdaysAsync(
        startDateTime,
        workdaysToAdd,
        cancellationToken);

    return Results.Ok(result);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
