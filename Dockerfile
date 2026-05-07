FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish ./WorkdayCalendar/src/WorkdayCalendar.Ui/WorkdayCalendar.Ui.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000

ENTRYPOINT ["dotnet", "WorkdayCalendar.Ui.dll"]