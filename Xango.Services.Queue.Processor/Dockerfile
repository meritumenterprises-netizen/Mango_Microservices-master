# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Xango.Services.Queue.Processor/Xango.Services.Queue.Processor.csproj", "Xango.Services.Queue.Processor/"]
RUN dotnet restore "./Xango.Services.Queue.Processor/Xango.Services.Queue.Processor.csproj"
COPY . .
WORKDIR "/src/Xango.Services.Queue.Processor"
RUN dotnet build "./Xango.Services.Queue.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Xango.Services.Queue.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y curl \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xango.Services.Queue.Processor.dll"]