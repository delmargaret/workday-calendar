# Workday Calendar

Calculates the datetime reached after moving forward or backward by a number of working days.

A working day is Monday to Friday, excluding configured fixed holidays and recurring holidays. The workday start and stop times are configurable.

## Projects

- `src/WorkdayCalendar.Domain` - domain models, aggregate, schedules, and workday calculation logic.
- `src/WorkdayCalendar.Application` - application services and repository abstractions.
- `src/WorkdayCalendar.Infrastructure` - JSON-specific repository implementation.
- `src/WorkdayCalendar.Console` - interactive console UI.
- `src/WorkdayCalendar.Ui` - Blazor UI and HTTP API with Swagger.
- `tests/*` - NUnit tests for domain, application, and infrastructure.

Swagger is available from the UI app at `/swagger`.

Test coverage is located at: `tests/CoverageReport/index.html`.

