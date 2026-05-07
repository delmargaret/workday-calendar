namespace WorkdayCalendar.Ui.Models;

/// <summary>
/// API model for a yearly recurring holiday.
/// </summary>
/// <param name="Day">The day of month.</param>
/// <param name="Month">The month.</param>
public record RecurringHolidayApiModel(int Day, int Month);
