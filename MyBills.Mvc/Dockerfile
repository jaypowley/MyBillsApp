# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MyBills.Mvc/MyBills.Mvc.csproj", "MyBills.Mvc/"]
COPY ["MyBills.Core/MyBills.Core.csproj", "MyBills.Core/"]
COPY ["MyBills.Domain/MyBills.Domain.csproj", "MyBills.Domain/"]
COPY ["MyBills.Services/MyBills.Services.csproj", "MyBills.Services/"]
COPY ["MyBills.Data/MyBills.Data.csproj", "MyBills.Data/"]
RUN dotnet restore "./MyBills.Mvc/MyBills.Mvc.csproj"
COPY . .
WORKDIR "/src/MyBills.Mvc"
RUN dotnet build "./MyBills.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyBills.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyBills.Mvc.dll"]