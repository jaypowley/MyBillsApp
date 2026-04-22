# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MyBills.Mvc/MyBills.Mvc.csproj", "MyBills.Mvc/"]
COPY ["MyBills.Core/MyBills.Core.csproj", "MyBills.Core/"]
COPY ["MyBills.Services/MyBills.Services.csproj", "MyBills.Services/"]
COPY ["MyBills.Data/MyBills.Data.csproj", "MyBills.Data/"]
COPY ["MyBills.Domain/MyBills.Domain.csproj", "MyBills.Domain/"]
RUN dotnet restore "MyBills.Mvc/MyBills.Mvc.csproj"

COPY . .
WORKDIR "/src/MyBills.Mvc"
RUN dotnet publish "MyBills.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyBills.Mvc.dll"]
