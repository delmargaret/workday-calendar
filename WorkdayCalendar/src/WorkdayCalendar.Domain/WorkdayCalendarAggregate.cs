using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Services;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Domain;

/// <summary>
/// Aggregate root for workday calendar configuration and calculations.
/// </summary>
public class WorkdayCalendarAggregate
{
    private readonly HolidaySchedule holidaySchedule;
    private WorkdaySchedule workdaySchedule;

    private WorkdayCalendarAggregate(WorkdaySchedule workdaySchedule, HolidaySchedule holidaySchedule)
    {
        this.workdaySchedule = workdaySchedule;
        this.holidaySchedule = holidaySchedule;
    }

    /// <summary>
    /// Creates a calendar with the default workday schedule and no holidays.
    /// </summary>
    /// <returns>A default workday calendar aggregate.</returns>
    public static WorkdayCalendarAggregate CreateDefault()
    {
        return new WorkdayCalendarAggregate(
            new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(16, 0)),
            new HolidaySchedule());
    }

    /// <summary>
    /// Creates a calendar from persisted state.
    /// </summary>
    /// <param name="workdaySchedule">The configured workday schedule.</param>
    /// <param name="holidays">The configured fixed holidays.</param>
    /// <param name="recurringHolidays">The configured recurring holidays.</param>
    /// <returns>A workday calendar aggregate.</returns>
    public static WorkdayCalendarAggregate Create(
        WorkdaySchedule workdaySchedule,
        IEnumerable<DateOnly> holidays,
        IEnumerable<RecurringHoliday> recurringHolidays)
    {
        var holidaySchedule = new HolidaySchedule();
        foreach (var holiday in holidays)
        {
            holidaySchedule.AddHoliday(holiday);
        }

        foreach (var recurringHoliday in recurringHolidays)
        {
            holidaySchedule.AddRecurringHoliday(recurringHoliday);
        }

        return new WorkdayCalendarAggregate(workdaySchedule, holidaySchedule);
    }

    /// <summary>
    /// Gets the configured workday schedule.
    /// </summary>
    public WorkdaySchedule WorkdaySchedule => workdaySchedule;

    /// <summary>
    /// Gets the configured fixed holidays.
    /// </summary>
    public IReadOnlySet<DateOnly> Holidays => holidaySchedule.Holidays;

    /// <summary>
    /// Gets the configured recurring holidays.
    /// </summary>
    public IReadOnlySet<RecurringHoliday> RecurringHolidays => holidaySchedule.RecurringHolidays;

    /// <summary>
    /// Sets the workday schedule.
    /// </summary>
    /// <param name="workdaySchedule">The workday schedule to use.</param>
    public void SetWorkdaySchedule(WorkdaySchedule workdaySchedule)
    {
        this.workdaySchedule = workdaySchedule;
    }

    /// <summary>
    /// Adds a fixed holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    public void AddHoliday(DateOnly date)
    {
        holidaySchedule.AddHoliday(date);
    }

    /// <summary>
    /// Removes a fixed holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    public void RemoveHoliday(DateOnly date)
    {
        holidaySchedule.RemoveHoliday(date);
    }

    /// <summary>
    /// Adds a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    public void AddRecurringHoliday(RecurringHoliday holiday)
    {
        holidaySchedule.AddRecurringHoliday(holiday);
    }

    /// <summary>
    /// Removes a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    public void RemoveRecurringHoliday(RecurringHoliday holiday)
    {
        holidaySchedule.RemoveRecurringHoliday(holiday);
    }

    /// <summary>
    /// Calculates the datetime reached after moving the specified number of workdays.
    /// </summary>
    /// <param name="startDateTime">The datetime to calculate from.</param>
    /// <param name="workdaysToAdd">The number of workdays to add. Negative values move backward.</param>
    /// <returns>The calculated datetime.</returns>
    public DateTime CalculateWorkdays(DateTime startDateTime, double workdaysToAdd)
    {
        return WorkdayCalculationService.CalculateWorkdays(workdaySchedule, holidaySchedule, startDateTime, workdaysToAdd);
    }
}
