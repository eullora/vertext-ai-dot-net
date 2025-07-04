FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AspNETCoreExample/AspNETCoreExample.csproj", "AspNETCoreExample/"]
RUN dotnet restore "./AspNETCoreExample/AspNETCoreExample.csproj"
COPY . .
WORKDIR "/src/AspNETCoreExample"
RUN dotnet build "./AspNETCoreExample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AspNETCoreExample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /mnt/efs

ENV ASPNETCORE_HTTP_PORTS=80
ENV GOOGLE_APPLICATION_CREDENTIALS="./google-service-account-key.json"
ENV GOOGLE_CLOUD_PROJECT="my-gcp-vertaxai-project"

ENTRYPOINT ["dotnet", "AspNETCoreExample.dll"]
#