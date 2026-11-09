
# Build the RaceDay API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore RaceDay.Api.csproj
RUN dotnet publish RaceDay.Api.csproj -c Release -o /app/publish --no-restore

# Run the RaceDay API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RaceDay.Api.dll"]
