# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7005

ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US:en
ENV LC_ALL=en_US.UTF-8
ENV CURRENCY=USD

ENV CertificateName="devcertificate.cer"
ENV DevCertificatePassword=Password1!
ENV AuthenticaionAPI="http://myhost:7002"
ENV CouponAPI="http://myhost:7001"
ENV ProductAPI="http://myhost:7005"
ENV ShoppingCartAPI="http://myhost:7003"
ENV OrderAPI="http://myhost:7004"
ENV InventoryAPI="http://myhost:7010"

ENV SQL_CONNECTION_STRING=Server="myhost,1433;Database=Xango_Order;User ID=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Xango.Services.ProductAPI/Xango.Services.ProductAPI.csproj", "Xango.Services.ProductAPI/"]
COPY ["Xango.Serrvices.Server.Utility/Xango.Serrvices.Server.Utility.csproj", "Xango.Serrvices.Server.Utility/"]
COPY ["Xango.Services.Client.Utility/Xango.Services.Client.Utility.csproj", "Xango.Services.Client.Utility/"]
COPY ["Xango.Services.Dto/Xango.Services.Dto.csproj", "Xango.Services.Dto/"]
RUN dotnet restore "./Xango.Services.ProductAPI/Xango.Services.ProductAPI.csproj"
COPY . .
WORKDIR "/src/Xango.Services.ProductAPI"
RUN dotnet build "./Xango.Services.ProductAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Xango.Services.ProductAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xango.Services.ProductAPI.dll"]