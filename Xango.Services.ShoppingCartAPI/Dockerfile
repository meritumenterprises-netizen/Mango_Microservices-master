# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7003

ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US:en
ENV LC_ALL=en_US.UTF-8
ENV CURRENCY=USD


ENV AuthenticaionAPI="https://myhost:7002"
ENV CouponAPI="https://myhost:7001"
ENV ProductAPI="https://myhost:7005"
ENV ShoppingCartAPI="https://myhost:7003"
ENV OrderAPI="https://myhost:7004"
ENV InventoryAPI="https://myhost:7010"

ENV SQL_CONNECTION_STRING=Server="myhost,1433;Database=Xango_Order;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Xango.Services.ShoppingCartAPI/Xango.Services.ShoppingCartAPI.csproj", "Xango.Services.ShoppingCartAPI/"]
COPY ["Xango.Serrvices.Server.Utility/Xango.Serrvices.Server.Utility.csproj", "Xango.Serrvices.Server.Utility/"]
COPY ["Xango.Services.Client.Utility/Xango.Services.Client.Utility.csproj", "Xango.Services.Client.Utility/"]
COPY ["Xango.Services.Dto/Xango.Services.Dto.csproj", "Xango.Services.Dto/"]
COPY ["Xango.Service.CouponAPI.Client/Xango.Service.CouponAPI.Client.csproj", "Xango.Service.CouponAPI.Client/"]
COPY ["Xango.Services.Interfaces/Xango.Services.Interfaces.csproj", "Xango.Services.Interfaces/"]
RUN dotnet restore "./Xango.Services.ShoppingCartAPI/Xango.Services.ShoppingCartAPI.csproj"
COPY . .
WORKDIR "/src/Xango.Services.ShoppingCartAPI"
RUN dotnet build "./Xango.Services.ShoppingCartAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Xango.Services.ShoppingCartAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xango.Services.ShoppingCartAPI.dll"]