namespace WorkdayCalendar.Ui.Models;

/// <summary>
/// API model for workday start and stop times.
/// </summary>
/// <param name="StartsAt">The time when a workday starts.</param>
/// <param name="StopsAt">The time when a workday stops.</param>
public record WorkdayScheduleApiModel(TimeOnly StartsAt, TimeOnly StopsAt);
