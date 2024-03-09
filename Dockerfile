From mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY Listjj/Listjj.csproj .
RUN dotnet restore "Listjj.csproj"
COPY . .
WORKDIR /src/Listjj/
RUN dotnet build "Listjj.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Listjj.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# for newer code:
ENV ASPNETCORE_HTTP_PORTS=80
#  for older upgraded code:
ENV ASPNETCORE_URLS=http://*:80

ENTRYPOINT ["dotnet", "Listjj.dll"]
