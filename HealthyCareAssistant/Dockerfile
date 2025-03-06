# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HealthyCareAssistant/HealthyCareAssistant.csproj", "HealthyCareAssistant/"]
COPY ["HealthyCareAssistant.Contract.Service/HealthyCareAssistant.Contract.Service.csproj", "HealthyCareAssistant.Contract.Service/"]
COPY ["HealthyCareAssistant.Contact.Repo/HealthyCareAssistant.Contact.Repo.csproj", "HealthyCareAssistant.Contact.Repo/"]
COPY ["HealthyCareAssistant.Core/HealthyCareAssistant.Core.csproj", "HealthyCareAssistant.Core/"]
COPY ["HealthyCareAssistant.ModelViews/HealthyCareAssistant.ModelViews.csproj", "HealthyCareAssistant.ModelViews/"]
COPY ["HealthyCareAssistant.Repo/HealthyCareAssistant.Repo.csproj", "HealthyCareAssistant.Repo/"]
COPY ["HealthyCareAssistant.Service/HealthyCareAssistant.Service.csproj", "HealthyCareAssistant.Service/"]
RUN dotnet restore "./HealthyCareAssistant/HealthyCareAssistant.csproj"
COPY . .
WORKDIR "/src/HealthyCareAssistant"
RUN dotnet build "./HealthyCareAssistant.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HealthyCareAssistant.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthyCareAssistant.dll"]