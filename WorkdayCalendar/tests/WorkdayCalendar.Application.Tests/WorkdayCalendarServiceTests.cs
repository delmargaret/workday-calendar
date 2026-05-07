using Moq;
using NUnit.Framework;
using WorkdayCalendar.Application.Abstractions;
using WorkdayCalendar.Application.Services;
using WorkdayCalendar.Domain;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Application.Tests;

public class WorkdayCalendarServiceTests
{
    private Mock<IWorkdayCalendarRepository> repository = null!;
    private WorkdayCalendarService service = null!;

    [SetUp]
    public void SetUp()
    {
        repository = CreateRepository();
        service = new WorkdayCalendarService(repository.Object);
    }

    [Test]
    public async Task AddHolidayAsync_updates_aggregate_and_repository()
    {
        var holiday = new DateOnly(2026, 6, 10);

        await service.AddHolidayAsync(holiday);

        var calendar = await repository.Object.GetAsync();
        Assert.That(calendar.Holidays, Does.Contain(holiday));
        repository.Verify(
            repository => repository.AddHolidayAsync(holiday, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task RemoveHolidayAsync_updates_aggregate_and_repository()
    {
        var holiday = new DateOnly(2026, 6, 10);
        SetRepositoryCalendar(
            WorkdayCalendarAggregate.Create(
                new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                [holiday],
                []));

        await service.RemoveHolidayAsync(holiday);

        var calendar = await repository.Object.GetAsync();
        Assert.That(calendar.Holidays, Does.Not.Contain(holiday));
        repository.Verify(
            repository => repository.RemoveHolidayAsync(holiday, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task SetWorkdayScheduleAsync_updates_aggregate_and_repository()
    {
        var schedule = new WorkdaySchedule(new TimeOnly(10, 0), new TimeOnly(18, 0));

        await service.SetWorkdayScheduleAsync(schedule);

        var calendar = await repository.Object.GetAsync();
        Assert.That(calendar.WorkdaySchedule, Is.EqualTo(schedule));
        repository.Verify(
            repository => repository.SaveWorkdayScheduleAsync(schedule, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task AddRecurringHolidayAsync_updates_aggregate_and_repository()
    {
        var holiday = new RecurringHoliday(23, 5);

        await service.AddRecurringHolidayAsync(holiday);

        var calendar = await repository.Object.GetAsync();
        Assert.That(calendar.RecurringHolidays, Does.Contain(holiday));
        repository.Verify(
            repository => repository.AddRecurringHolidayAsync(holiday, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task RemoveRecurringHolidayAsync_updates_aggregate_and_repository()
    {
        var holiday = new RecurringHoliday(23, 5);
        SetRepositoryCalendar(
            WorkdayCalendarAggregate.Create(
                new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(16, 0)),
                [],
                [holiday]));

        await service.RemoveRecurringHolidayAsync(holiday);

        var calendar = await repository.Object.GetAsync();
        Assert.That(calendar.RecurringHolidays, Does.Not.Contain(holiday));
        repository.Verify(
            repository => repository.RemoveRecurringHolidayAsync(holiday, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetWorkdayCalendarAsync_returns_calendar()
    {
        SetRepositoryCalendar(
            WorkdayCalendarAggregate.Create(
                new WorkdaySchedule(new TimeOnly(9, 0), new TimeOnly(17, 0)),
                [new DateOnly(2026, 12, 02), new DateOnly(2026, 1, 23)],
                [new RecurringHoliday(25, 12), new RecurringHoliday(7, 1)]));

        var calendar = await service.GetWorkdayCalendarAsync();

        Assert.That(calendar.WorkdaySchedule, Is.EqualTo(new WorkdaySchedule(new TimeOnly(9, 0), new TimeOnly(17, 0))));
        Assert.That(calendar.Holidays, Is.EqualTo(new[] { new DateOnly(2026, 1, 23), new DateOnly(2026, 12, 02) }));
        Assert.That(calendar.RecurringHolidays, Is.EqualTo(new[] { new RecurringHoliday(7, 1), new RecurringHoliday(25, 12) }));
    }

    [Test]
    public async Task CalculateWorkdaysAsync_uses_current_calendar_configuration()
    {
        SetRepositoryCalendar(
            WorkdayCalendarAggregate.Create(
                new WorkdaySchedule(new TimeOnly(10, 0), new TimeOnly(18, 0)),
                [new DateOnly(2026, 6, 11)],
                [new RecurringHoliday(12, 6)]));

        var result = await service.CalculateWorkdaysAsync(
            new DateTime(2026, 6, 10, 9, 0, 0, DateTimeKind.Unspecified),
            1.5);

        Assert.That(result, Is.EqualTo(new DateTime(2026, 6, 15, 14, 0, 0, DateTimeKind.Unspecified)));
    }

    private static Mock<IWorkdayCalendarRepository> CreateRepository()
    {
        var repository = new Mock<IWorkdayCalendarRepository>();
        repository
            .Setup(repository => repository.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(WorkdayCalendarAggregate.CreateDefault());
        return repository;
    }

    private void SetRepositoryCalendar(WorkdayCalendarAggregate calendar)
    {
        repository
            .Setup(repository => repository.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);
    }
}
