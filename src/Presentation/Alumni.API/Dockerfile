FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Clean Architecture katmanlarındaki projelerin kopyalanması
COPY ["src/Core/Alumni.Domain/Alumni.Domain.csproj", "src/Core/Alumni.Domain/"]
COPY ["src/Core/Alumni.Application/Alumni.Application.csproj", "src/Core/Alumni.Application/"]
COPY ["src/Infrastructure/Alumni.Persistence/Alumni.Persistence.csproj", "src/Infrastructure/Alumni.Persistence/"]
COPY ["src/Infrastructure/Alumni.Infrastructure/Alumni.Infrastructure.csproj", "src/Infrastructure/Alumni.Infrastructure/"]
COPY ["src/Presentation/Alumni.API/Alumni.API.csproj", "src/Presentation/Alumni.API/"]

# Bağımlılıkların restore edilmesi
RUN dotnet restore "src/Presentation/Alumni.API/Alumni.API.csproj"

# Kalan tüm kaynak kodların kopyalanması ve derleme/publish işlemleri
COPY . .
WORKDIR "/src/src/Presentation/Alumni.API"
RUN dotnet build "Alumni.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Alumni.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alumni.API.dll"]
