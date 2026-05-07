# Workday Calendar

Workday Calendar calculates the datetime reached after moving forward or backward by a number of working days.

A working day is Monday to Friday, excluding configured fixed holidays and recurring yearly holidays. Workday start and stop times are configurable.

## Features

- Calculate forward and backward working-day offsets.
- Support fractional working days such as `0.25` and `-5.5`.
- Skip weekends, fixed holidays, and recurring holidays.
- Configure workday start and stop times.
- Use either the interactive console app or the Blazor UI.

## Projects

| Project                              | Purpose                                                             |
| ------------------------------------ | ------------------------------------------------------------------- |
| `src/WorkdayCalendar.Domain`         | Domain models, aggregate, schedules, and workday calculation logic. |
| `src/WorkdayCalendar.Application`    | Application services and repository abstractions.                   |
| `src/WorkdayCalendar.Infrastructure` | JSON-specific repository implementation.                            |
| `src/WorkdayCalendar.Console`        | Interactive console UI.                                             |
| `src/WorkdayCalendar.Ui`             | Blazor UI and HTTP API with Swagger.                                |
| `tests/*`                            | NUnit tests for domain, application, and infrastructure.            |

Swagger is available from the UI app at `/swagger`.

Test coverage is located at: `WorkdayCalendar/tests/CoverageReport/index.htm`

## Demo Deployment

The UI app is deployed to Render for testing purposes.

| Link     | URL                                                      |
| -------- | -------------------------------------------------------- |
| Main app | https://workday-calendar.onrender.com/                   |
| Swagger  | https://workday-calendar.onrender.com/swagger/index.html |

