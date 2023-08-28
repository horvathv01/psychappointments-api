FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PsychAppointments-API.csproj", "./"]
RUN dotnet restore "PsychAppointments-API.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PsychAppointments-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PsychAppointments-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PsychAppointments-API.dll"]
