using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using WorkdayCalendar.Application.Services;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;
using WorkdayCalendar.Infrastructure;

var services = new ServiceCollection();
services.AddScoped<IWorkdayCalendarService, WorkdayCalendarService>();
services.AddJsonCalendarInfrastructure();

await using var provider = services.BuildServiceProvider();
var calendarService = provider.GetRequiredService<IWorkdayCalendarService>();

Console.WriteLine("Workday Calendar");

const string DateFormat = "dd-MM-yyyy";
const string RecurringDateFormat = "dd-MM";
const string DateTimeFormat = "dd-MM-yyyy HH:mm";
const string TimeFormat = "HH:mm";

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1. Show configuration");
    Console.WriteLine("2. Set workday hours");
    Console.WriteLine("3. Add fixed holiday");
    Console.WriteLine("4. Remove fixed holiday");
    Console.WriteLine("5. Add recurring holiday");
    Console.WriteLine("6. Remove recurring holiday");
    Console.WriteLine("7. Calculate workdays");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    try
    {
        running = await HandleChoiceAsync(Console.ReadLine(), calendarService);
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Error: {exception.Message}");
    }
}

static async Task<bool> HandleChoiceAsync(string? choice, IWorkdayCalendarService calendarService)
{
    switch (choice)
    {
        case "1":
            await ShowCalendarAsync(calendarService);
            return true;
        case "2":
            await calendarService.SetWorkdayScheduleAsync(ReadWorkdaySchedule());
            Console.WriteLine("Workday hours saved.");
            return true;
        case "3":
            await calendarService.AddHolidayAsync(ReadDate($"Holiday date ({DateFormat}): "));
            Console.WriteLine("Holiday saved.");
            return true;
        case "4":
            await calendarService.RemoveHolidayAsync(ReadDate($"Holiday date ({DateFormat}): "));
            Console.WriteLine("Holiday removed.");
            return true;
        case "5":
            await calendarService.AddRecurringHolidayAsync(ReadRecurringHoliday());
            Console.WriteLine("Recurring holiday saved.");
            return true;
        case "6":
            await calendarService.RemoveRecurringHolidayAsync(ReadRecurringHoliday());
            Console.WriteLine("Recurring holiday removed.");
            return true;
        case "7":
            await CalculateAsync(calendarService);
            return true;
        case "0":
            return false;
        default:
            Console.WriteLine("Unknown choice.");
            return true;
    }
}

static async Task ShowCalendarAsync(IWorkdayCalendarService calendarService)
{
    var calendar = await calendarService.GetWorkdayCalendarAsync();

    Console.WriteLine();
    Console.WriteLine($"Workday hours: {FormatTime(calendar.WorkdaySchedule.StartsAt)}-{FormatTime(calendar.WorkdaySchedule.StopsAt)}");
    Console.WriteLine("Fixed holidays:");
    foreach (var holiday in calendar.Holidays)
    {
        Console.WriteLine($"  {FormatDate(holiday)}");
    }

    Console.WriteLine("Recurring holidays:");
    foreach (var holiday in calendar.RecurringHolidays)
    {
        Console.WriteLine($"  {FormatRecurringHoliday(holiday)}");
    }
}

static async Task CalculateAsync(IWorkdayCalendarService calendarService)
{
    var start = ReadDateTime($"Start datetime ({DateTimeFormat}): ");
    var workdaysToAdd = ReadDouble("Workdays to add, negative for backwards: ");
    var result = await calendarService.CalculateWorkdaysAsync(start, workdaysToAdd);
    Console.WriteLine($"Result: {FormatDateTime(result)}");
}

static WorkdaySchedule ReadWorkdaySchedule()
{
    var start = ReadTime($"Workday start ({TimeFormat}): ");
    var end = ReadTime($"Workday end ({TimeFormat}): ");
    return new WorkdaySchedule(start, end);
}

static RecurringHoliday ReadRecurringHoliday()
{
    Console.Write($"Recurring date ({RecurringDateFormat}): ");
    var parsed = DateOnly.ParseExact(Console.ReadLine() ?? string.Empty, RecurringDateFormat, CultureInfo.InvariantCulture);
    return new RecurringHoliday(parsed.Day, parsed.Month);
}

static DateOnly ReadDate(string prompt)
{
    Console.Write(prompt);
    return DateOnly.ParseExact(Console.ReadLine() ?? string.Empty, DateFormat, CultureInfo.InvariantCulture);
}

static DateTime ReadDateTime(string prompt)
{
    Console.Write(prompt);
    return DateTime.ParseExact(Console.ReadLine() ?? string.Empty, DateTimeFormat, CultureInfo.InvariantCulture);
}

static TimeOnly ReadTime(string prompt)
{
    Console.Write(prompt);
    return TimeOnly.ParseExact(Console.ReadLine() ?? string.Empty, TimeFormat, CultureInfo.InvariantCulture);
}

static double ReadDouble(string prompt)
{
    Console.Write(prompt);
    return double.Parse(Console.ReadLine() ?? string.Empty, CultureInfo.InvariantCulture);
}

static string FormatTime(TimeOnly time)
{
    return time.ToString(TimeFormat, CultureInfo.InvariantCulture);
}

static string FormatDate(DateOnly date)
{
    return date.ToString(DateFormat, CultureInfo.InvariantCulture);
}

static string FormatDateTime(DateTime dateTime)
{
    return dateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
}

static string FormatRecurringHoliday(RecurringHoliday holiday)
{
    return $"{holiday.Day:00}-{holiday.Month:00}";
}
