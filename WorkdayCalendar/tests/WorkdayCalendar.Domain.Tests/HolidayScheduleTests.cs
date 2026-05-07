using NUnit.Framework;
using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Domain.Tests;

public class HolidayScheduleTests
{
    [Test]
    public void IsHoliday_returns_true_for_fixed_holiday()
    {
        var schedule = new HolidaySchedule();
        var holiday = new DateOnly(2026, 6, 10);

        schedule.AddHoliday(holiday);

        Assert.That(schedule.IsHoliday(holiday), Is.True);
    }

    [Test]
    public void IsHoliday_returns_true_for_recurring_holiday()
    {
        var schedule = new HolidaySchedule();

        schedule.AddRecurringHoliday(new RecurringHoliday(17, 5));

        Assert.That(schedule.IsHoliday(new DateOnly(2026, 5, 17)), Is.True);
        Assert.That(schedule.IsHoliday(new DateOnly(2030, 5, 17)), Is.True);
    }

    [Test]
    public void RemoveHoliday_removes_fixed_holiday()
    {
        var schedule = new HolidaySchedule();
        var holiday = new DateOnly(2026, 6, 10);

        schedule.AddHoliday(holiday);
        schedule.RemoveHoliday(holiday);

        Assert.That(schedule.IsHoliday(holiday), Is.False);
    }

    [Test]
    public void RemoveRecurringHoliday_removes_recurring_holiday()
    {
        var schedule = new HolidaySchedule();
        var holiday = new RecurringHoliday(17, 5);

        schedule.AddRecurringHoliday(holiday);
        schedule.RemoveRecurringHoliday(holiday);

        Assert.That(schedule.IsHoliday(new DateOnly(2026, 5, 17)), Is.False);
    }
}
