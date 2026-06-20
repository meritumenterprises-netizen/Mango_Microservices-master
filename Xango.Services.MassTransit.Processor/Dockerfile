# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Xango.Services.MassTransit.Processor/Xango.Services.MassTransit.Processor.csproj", "Xango.Services.MassTransit.Processor/"]
COPY ["Xango.Service.AuthenticationAPI.Client/Xango.Service.AuthenticationAPI.Client.csproj", "Xango.Service.AuthenticationAPI.Client/"]
COPY ["Xango.Service.OrderAPI.Client/Xango.Service.OrderAPI.Client.csproj", "Xango.Service.OrderAPI.Client/"]
COPY ["Xango.Services.Client.Utility/Xango.Services.Client.Utility.csproj", "Xango.Services.Client.Utility/"]
COPY ["Xango.Services.Dto/Xango.Models.Dto.csproj", "Xango.Services.Dto/"]
COPY ["Xango.Services.Interfaces/Xango.Services.Interfaces.csproj", "Xango.Services.Interfaces/"]
COPY ["Xango.Services.Server.Utility/Xango.Services.Server.Utility.csproj", "Xango.Services.Server.Utility/"]
RUN dotnet restore "./Xango.Services.MassTransit.Processor/Xango.Services.MassTransit.Processor.csproj"
COPY . .
WORKDIR "/src/Xango.Services.MassTransit.Processor"
RUN dotnet build "./Xango.Services.MassTransit.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Xango.Services.MassTransit.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xango.Services.MassTransit.Processor.dll"]
