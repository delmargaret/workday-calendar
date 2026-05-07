namespace WorkdayCalendar.Ui.Models;

/// <summary>
/// API model for workday calendar configuration.
/// </summary>
/// <param name="WorkdaySchedule">The configured workday schedule.</param>
/// <param name="Holidays">The configured fixed holidays.</param>
/// <param name="RecurringHolidays">The configured recurring holidays.</param>
public record WorkdayCalendarApiModel(
    WorkdayScheduleApiModel WorkdaySchedule,
    IReadOnlyCollection<DateOnly> Holidays,
    IReadOnlyCollection<RecurringHolidayApiModel> RecurringHolidays);
