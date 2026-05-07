using NUnit.Framework;
using WorkdayCalendar.Application.Services;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;
using WorkdayCalendar.Infrastructure.Json;

namespace WorkdayCalendar.Infrastructure.Tests;

public class JsonWorkdayCalendarRepositoryTests
{
    private string filePath = null!;

    [SetUp]
    public void SetUp()
    {
        filePath = Path.Combine(Path.GetTempPath(), $"workday-calendar-{Guid.NewGuid():N}.json");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task Repository_creates_default_calendar_file_when_file_does_not_exist()
    {
        var repository = new JsonWorkdayCalendarRepository(filePath);
        var calendar = await repository.GetAsync();

        Assert.That(File.Exists(filePath), Is.True);
        Assert.That(calendar.WorkdaySchedule.StartsAt, Is.EqualTo(new TimeOnly(8, 0)));
        Assert.That(calendar.WorkdaySchedule.StopsAt, Is.EqualTo(new TimeOnly(16, 0)));
    }

    [Test]
    public async Task Repository_persists_calendar_changes_to_json()
    {
        var service = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));
        var holiday = new DateOnly(2026, 6, 10);
        var recurringHoliday = new RecurringHoliday(23, 5);
        var schedule = new WorkdaySchedule(new TimeOnly(9, 0), new TimeOnly(15, 30));

        await service.SetWorkdayScheduleAsync(schedule);
        await service.AddHolidayAsync(holiday);
        await service.AddRecurringHolidayAsync(recurringHoliday);

        var reloadedService = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));
        var reloadedCalendar = await reloadedService.GetWorkdayCalendarAsync();

        Assert.That(reloadedCalendar.WorkdaySchedule, Is.EqualTo(schedule));
        Assert.That(reloadedCalendar.Holidays, Does.Contain(holiday));
        Assert.That(reloadedCalendar.RecurringHolidays, Does.Contain(recurringHoliday));
    }

    [Test]
    public async Task Repository_persists_removed_holidays_to_json()
    {
        var service = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));
        var holiday = new DateOnly(2026, 6, 10);
        var recurringHoliday = new RecurringHoliday(23, 5);

        await service.AddHolidayAsync(holiday);
        await service.AddRecurringHolidayAsync(recurringHoliday);
        await service.RemoveHolidayAsync(holiday);
        await service.RemoveRecurringHolidayAsync(recurringHoliday);

        var reloadedService = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));
        var reloadedCalendar = await reloadedService.GetWorkdayCalendarAsync();

        Assert.That(reloadedCalendar.Holidays, Does.Not.Contain(holiday));
        Assert.That(reloadedCalendar.RecurringHolidays, Does.Not.Contain(recurringHoliday));
    }

    [Test]
    public async Task Reloaded_calendar_is_used_for_calculation()
    {
        var service = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));

        await service.SetWorkdayScheduleAsync(new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(16, 0)));
        await service.AddRecurringHolidayAsync(new RecurringHoliday(17, 5));
        await service.AddHolidayAsync(new DateOnly(2004, 5, 27));

        var reloadedService = new WorkdayCalendarService(new JsonWorkdayCalendarRepository(filePath));
        var result = await reloadedService.CalculateWorkdaysAsync(
            new DateTime(2004, 5, 24, 18, 5, 0, DateTimeKind.Unspecified),
            -5.5);

        Assert.That(result, Is.EqualTo(new DateTime(2004, 5, 14, 12, 0, 0, DateTimeKind.Unspecified)));
    }
}
