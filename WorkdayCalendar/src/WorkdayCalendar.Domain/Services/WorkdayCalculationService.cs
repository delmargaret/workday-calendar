using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Domain.Services;

/// <summary>
/// Calculates datetimes by moving forward or backward through workday time.
/// </summary>
internal static class WorkdayCalculationService
{
    private enum CountingDirection
    {
        Backward = -1,
        Forward = 1
    }

    /// <summary>
    /// Calculates the datetime reached after moving the specified number of workdays.
    /// </summary>
    /// <param name="workdaySchedule">The workday schedule that defines workday hours.</param>
    /// <param name="holidaySchedule">The holiday schedule that defines non-working dates.</param>
    /// <param name="startDateTime">The datetime to calculate from.</param>
    /// <param name="workdaysToAdd">The number of workdays to add. Negative values move backward.</param>
    /// <returns>The calculated datetime.</returns>
    public static DateTime CalculateWorkdays(
        WorkdaySchedule workdaySchedule,
        HolidaySchedule holidaySchedule,
        DateTime startDateTime,
        double workdaysToAdd)
    {
        if (workdaysToAdd == 0d)
        {
            return ResolveNearestValidWorkdayTime(
                workdaySchedule,
                holidaySchedule,
                startDateTime,
                CountingDirection.Forward);
        }

        var totalWorkdayMinutes = ConvertWorkdaysToMinutes(workdaySchedule, workdaysToAdd);
        var direction = workdaysToAdd > 0
            ? CountingDirection.Forward
            : CountingDirection.Backward;

        return AddWorkdayMinutes(
            workdaySchedule,
            holidaySchedule,
            startDateTime,
            totalWorkdayMinutes,
            direction);
    }

    private static DateTime ResolveNearestValidWorkdayTime(
        WorkdaySchedule workdaySchedule,
        HolidaySchedule holidaySchedule,
        DateTime startDateTime,
        CountingDirection direction)
    {
        var isForward = direction == CountingDirection.Forward;
        var startDate = DateOnly.FromDateTime(startDateTime);
        var startTime = TimeOnly.FromDateTime(startDateTime);

        var isPastWorkdayWindow = isForward
            ? startTime > workdaySchedule.StopsAt
            : startTime < workdaySchedule.StartsAt;

        if (IsNonWorkday(holidaySchedule, startDate) || isPastWorkdayWindow)
        {
            return FindNextValidWorkdayTime(
                workdaySchedule,
                holidaySchedule,
                startDate.AddDays((int)direction),
                direction);
        }

        var isBeforeWorkdayWindow = isForward
            ? startTime < workdaySchedule.StartsAt
            : startTime > workdaySchedule.StopsAt;

        if (isBeforeWorkdayWindow)
        {
            var currentWorkdayCountingStartTime = isForward
                ? workdaySchedule.StartsAt
                : workdaySchedule.StopsAt;
            return startDate.ToDateTime(currentWorkdayCountingStartTime);
        }

        return startDateTime;
    }

    private static DateTime AddWorkdayMinutes(
        WorkdaySchedule workdaySchedule,
        HolidaySchedule holidaySchedule,
        DateTime startDateTime,
        long minutesToAdd,
        CountingDirection direction)
    {
        var current = ResolveNearestValidWorkdayTime(workdaySchedule, holidaySchedule, startDateTime, direction);
        var isForward = direction == CountingDirection.Forward;

        while (minutesToAdd > 0)
        {
            var currentDate = DateOnly.FromDateTime(current);
            var currentWorkdayStart = currentDate.ToDateTime(workdaySchedule.StartsAt);
            var currentWorkdayEnd = currentDate.ToDateTime(workdaySchedule.StopsAt);
            var minutesAvailableOnCurrentDate = isForward
                ? (long)Math.Floor((currentWorkdayEnd - current).TotalMinutes)
                : (long)Math.Floor((current - currentWorkdayStart).TotalMinutes);

            if (minutesToAdd <= minutesAvailableOnCurrentDate)
            {
                return current.AddMinutes((double)minutesToAdd * (int)direction);
            }

            minutesToAdd -= minutesAvailableOnCurrentDate;
            current = FindNextValidWorkdayTime(
                workdaySchedule,
                holidaySchedule,
                currentDate.AddDays((int)direction),
                direction);
        }

        return current;
    }

    private static DateTime FindNextValidWorkdayTime(
        WorkdaySchedule workdaySchedule,
        HolidaySchedule holidaySchedule,
        DateOnly searchFrom,
        CountingDirection direction)
    {
        var date = searchFrom;
        while (IsNonWorkday(holidaySchedule, date))
        {
            date = date.AddDays((int)direction);
        }

        var countingStartTime = direction == CountingDirection.Forward
            ? workdaySchedule.StartsAt
            : workdaySchedule.StopsAt;
        return date.ToDateTime(countingStartTime);
    }

    private static bool IsNonWorkday(HolidaySchedule holidaySchedule, DateOnly date)
    {
        return holidaySchedule.IsHoliday(date) ||
            date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    private static long ConvertWorkdaysToMinutes(WorkdaySchedule workdaySchedule, double workdaysToAdd)
    {
        var workdayMinutes = workdaySchedule.WorkdayDuration.TotalMinutes;
        return (long)Math.Floor(Math.Abs(workdaysToAdd) * workdayMinutes);
    }
}
