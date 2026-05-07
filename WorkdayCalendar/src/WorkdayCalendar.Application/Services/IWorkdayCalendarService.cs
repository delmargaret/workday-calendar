using WorkdayCalendar.Application.Models;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Application.Services;

/// <summary>
/// Coordinates workday calendar use cases.
/// </summary>
public interface IWorkdayCalendarService
{
    /// <summary>
    /// Gets the current workday calendar configuration.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The current workday calendar configuration.</returns>
    Task<WorkdayCalendarSettings> GetWorkdayCalendarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the workday schedule used for calculations.
    /// </summary>
    /// <param name="workdaySchedule">The workday schedule to use.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetWorkdayScheduleAsync(WorkdaySchedule workdaySchedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single-date holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddHolidayAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single-date holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveHolidayAsync(DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the datetime reached after moving the specified number of workdays.
    /// </summary>
    /// <param name="startDateTime">The datetime to calculate from.</param>
    /// <param name="workdaysToAdd">The number of workdays to add. Negative values move backward.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The calculated datetime.</returns>
    Task<DateTime> CalculateWorkdaysAsync(
        DateTime startDateTime,
        double workdaysToAdd,
        CancellationToken cancellationToken = default);
}
