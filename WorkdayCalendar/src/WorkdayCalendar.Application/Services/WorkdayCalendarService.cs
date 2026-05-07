using WorkdayCalendar.Application.Abstractions;
using WorkdayCalendar.Application.Models;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Application.Services;

/// <inheritdoc />
public class WorkdayCalendarService(IWorkdayCalendarRepository repository) : IWorkdayCalendarService
{
    /// <inheritdoc />
    public async Task<WorkdayCalendarSettings> GetWorkdayCalendarAsync(CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        return new WorkdayCalendarSettings(
            calendar.WorkdaySchedule,
            [.. calendar.Holidays.Order()],
            [.. calendar.RecurringHolidays.OrderBy(holiday => holiday.Month).ThenBy(holiday => holiday.Day)]);
    }

    /// <inheritdoc />
    public async Task SetWorkdayScheduleAsync(WorkdaySchedule workdaySchedule, CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        calendar.SetWorkdaySchedule(workdaySchedule);
        await repository.SaveWorkdayScheduleAsync(workdaySchedule, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddHolidayAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        calendar.AddHoliday(date);
        await repository.AddHolidayAsync(date, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveHolidayAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        calendar.RemoveHoliday(date);
        await repository.RemoveHolidayAsync(date, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        calendar.AddRecurringHoliday(holiday);
        await repository.AddRecurringHolidayAsync(holiday, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        calendar.RemoveRecurringHoliday(holiday);
        await repository.RemoveRecurringHolidayAsync(holiday, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DateTime> CalculateWorkdaysAsync(
        DateTime startDateTime,
        double workdaysToAdd,
        CancellationToken cancellationToken = default)
    {
        var calendar = await repository.GetAsync(cancellationToken);
        return calendar.CalculateWorkdays(startDateTime, workdaysToAdd);
    }
}
