# =========================
# Stage 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["JobTracker.Api/JobTracker.Api.csproj", "JobTracker.Api/"]
COPY ["JobTracker.Application/JobTracker.Application.csproj", "JobTracker.Application/"]
COPY ["JobTracker.Domain/JobTracker.Domain.csproj", "JobTracker.Domain/"]
COPY ["JobTracker.Infrastructure/JobTracker.Infrastructure.csproj", "JobTracker.Infrastructure/"]

RUN dotnet restore "JobTracker.Api/JobTracker.Api.csproj"

COPY . .

WORKDIR "/src/JobTracker.Api"

RUN dotnet build "JobTracker.Api.csproj" -c Release -o /app/build


# =========================
# Stage 2: Publish
# =========================
FROM build AS publish

RUN dotnet publish "JobTracker.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Stage 3: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobTracker.Api.dll"]