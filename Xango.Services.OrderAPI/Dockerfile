# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US:en
ENV LC_ALL=en_US.UTF-8
ENV CURRENCY=USD

ENV AuthenticaionAPI="http://auth-api.default.svc.cluster.local"
ENV CouponAPI="http://coupon-api.default.svc.cluster.local"
ENV ProductAPI="http://product-api.default.svc.cluster.local"
ENV ShoppingCartAPI="http://shoppingcart-api.default.svc.cluster.local"
ENV OrderAPI="http://order-api.default.svc.cluster.local"
ENV InventoryAPI="http://inventory-api.default.svc.cluster.local"

ENV SQL_CONNECTION_STRING=Server="tcp:mssql.default.svc.cluster.local,1433;Database=Xango_Order;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Xango.Services.OrderAPI/Xango.Services.OrderAPI.csproj", "Xango.Services.OrderAPI/"]
COPY ["Xango.Serrvices.Server.Utility/Xango.Serrvices.Server.Utility.csproj", "Xango.Serrvices.Server.Utility/"]
COPY ["Xango.Services.Client.Utility/Xango.Services.Client.Utility.csproj", "Xango.Services.Client.Utility/"]
COPY ["Xango.Services.Dto/Xango.Services.Dto.csproj", "Xango.Services.Dto/"]
COPY ["Xango.Service.InventoryAPI.Client/Xango.Service.InventoryAPI.Client.csproj", "Xango.Service.InventoryAPI.Client/"]
COPY ["Xango.Services.Interfaces/Xango.Services.Interfaces.csproj", "Xango.Services.Interfaces/"]
RUN dotnet restore "./Xango.Services.OrderAPI/Xango.Services.OrderAPI.csproj"
COPY . .
WORKDIR "/src/Xango.Services.OrderAPI"
RUN dotnet build "./Xango.Services.OrderAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Xango.Services.OrderAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xango.Services.OrderAPI.dll"]