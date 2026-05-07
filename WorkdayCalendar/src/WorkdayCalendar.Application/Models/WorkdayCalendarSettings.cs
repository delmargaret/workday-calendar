using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Application.Models;

/// <summary>
/// Represents the current calendar configuration.
/// </summary>
/// <param name="WorkdaySchedule">The schedule that defines workday start and stop times.</param>
/// <param name="Holidays">The configured single-date holidays.</param>
/// <param name="RecurringHolidays">The configured yearly recurring holidays.</param>
public record WorkdayCalendarSettings(
    WorkdaySchedule WorkdaySchedule,
    IReadOnlyCollection<DateOnly> Holidays,
    IReadOnlyCollection<RecurringHoliday> RecurringHolidays);
