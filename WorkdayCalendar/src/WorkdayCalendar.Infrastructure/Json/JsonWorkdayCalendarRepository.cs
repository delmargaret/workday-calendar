using System.Text.Json;
using WorkdayCalendar.Application.Abstractions;
using WorkdayCalendar.Domain;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Infrastructure.Json;

/// <summary>
/// Stores workday calendar data in a JSON file.
/// </summary>
public sealed class JsonWorkdayCalendarRepository : IWorkdayCalendarRepository
{
    private readonly string filePath;
    private readonly WorkdayCalendarAggregate calendar;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonWorkdayCalendarRepository" /> class.
    /// </summary>
    /// <param name="file">The JSON file path.</param>
    public JsonWorkdayCalendarRepository(string file)
    {
        filePath = Path.GetFullPath(file);
        calendar = LoadCalendar();
    }

    /// <inheritdoc />
    public Task<WorkdayCalendarAggregate> GetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(calendar);
    }

    /// <inheritdoc />
    public Task SaveWorkdayScheduleAsync(WorkdaySchedule workdaySchedule, CancellationToken cancellationToken = default)
    {
        return SaveCalendarAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddHolidayAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return SaveCalendarAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveHolidayAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return SaveCalendarAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default)
    {
        return SaveCalendarAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveRecurringHolidayAsync(RecurringHoliday holiday, CancellationToken cancellationToken = default)
    {
        return SaveCalendarAsync(cancellationToken);
    }

    private WorkdayCalendarAggregate LoadCalendar()
    {
        if (!File.Exists(filePath))
        {
            var defaultCalendar = WorkdayCalendarAggregate.CreateDefault();
            Save(defaultCalendar);
            return defaultCalendar;
        }

        using var stream = File.OpenRead(filePath);
        var document = JsonSerializer.Deserialize<WorkdayCalendarDto>(stream);

        return document is null ? WorkdayCalendarAggregate.CreateDefault() : document.ToDomain();
    }

    private async Task SaveCalendarAsync(CancellationToken cancellationToken)
    {
        EnsureDirectoryExists();

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(
            stream,
            WorkdayCalendarDto.FromDomain(calendar),
            options: null,
            cancellationToken);
    }

    private void Save(WorkdayCalendarAggregate calendar)
    {
        EnsureDirectoryExists();

        using var stream = File.Create(filePath);
        JsonSerializer.Serialize(stream, WorkdayCalendarDto.FromDomain(calendar));
    }

    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private record WorkdayCalendarDto(
        WorkdayScheduleDto? WorkdaySchedule,
        IReadOnlyCollection<DateOnly>? Holidays,
        IReadOnlyCollection<RecurringHolidayDto>? RecurringHolidays)
    {
        public static WorkdayCalendarDto FromDomain(WorkdayCalendarAggregate calendar)
        {
            var recurringHolidays = calendar.RecurringHolidays
                .OrderBy(holiday => holiday.Month)
                .ThenBy(holiday => holiday.Day)
                .Select(holiday => new RecurringHolidayDto(holiday.Day, holiday.Month))
                .ToArray();

            return new WorkdayCalendarDto(
                WorkdayScheduleDto.FromDomain(calendar.WorkdaySchedule),
                [.. calendar.Holidays.Order()],
                recurringHolidays);
        }

        public WorkdayCalendarAggregate ToDomain()
        {
            var defaultCalendar = WorkdayCalendarAggregate.CreateDefault();

            return WorkdayCalendarAggregate.Create(
                WorkdaySchedule?.ToDomain() ?? defaultCalendar.WorkdaySchedule,
                Holidays ?? [],
                RecurringHolidays?.Select(holiday => new RecurringHoliday(holiday.Day, holiday.Month)) ?? []);
        }
    }

    private record WorkdayScheduleDto(TimeOnly StartsAt, TimeOnly StopsAt)
    {
        public static WorkdayScheduleDto FromDomain(WorkdaySchedule workdaySchedule)
        {
            return new WorkdayScheduleDto(workdaySchedule.StartsAt, workdaySchedule.StopsAt);
        }

        public WorkdaySchedule ToDomain()
        {
            return new WorkdaySchedule(StartsAt, StopsAt);
        }
    }

    private record RecurringHolidayDto(int Day, int Month);
}
