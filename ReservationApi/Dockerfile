#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReservationApi/ReservationApi.csproj", "ReservationApi/"]
COPY ["ReservationApi/ReservationSystem.Common/ReservationSystem.Common.csproj", "ReservationApi/ReservationSystem.Common/"]
COPY ["ReservationApi/ReservationSystem.Domain/ReservationSystem.Domain.csproj", "ReservationApi/ReservationSystem.Domain/"]
COPY ["ReservationApi/ReservationSystem.Infrastructure/ReservationSystem.Infrastructure.csproj", "ReservationApi/ReservationSystem.Infrastructure/"]
RUN dotnet restore "ReservationApi/ReservationApi.csproj"
COPY . .
WORKDIR "/src/ReservationApi"
RUN dotnet build "ReservationApi.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ReservationApi.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReservationApi.dll"]

