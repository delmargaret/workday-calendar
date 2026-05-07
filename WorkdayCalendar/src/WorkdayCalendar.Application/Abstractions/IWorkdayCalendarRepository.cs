using WorkdayCalendar.Domain;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Application.Abstractions;

/// <summary>
/// Persists and retrieves the workday calendar information.
/// </summary>
public interface IWorkdayCalendarRepository
{
    /// <summary>
    /// Gets the current workday calendar info.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The current workday calendar aggregate.</returns>
    Task<WorkdayCalendarAggregate> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the configured workday schedule.
    /// </summary>
    /// <param name="workdaySchedule">The workday schedule to save.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveWorkdayScheduleAsync(WorkdaySchedule workdaySchedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single-date holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task AddHolidayAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single-date holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task RemoveHolidayAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task AddRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task RemoveRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default);
}
