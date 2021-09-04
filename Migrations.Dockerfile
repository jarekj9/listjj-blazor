 
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY ["Listjj/Listjj.csproj", "./"]

RUN dotnet tool install --global dotnet-ef

RUN dotnet restore "./Listjj.csproj"
COPY . .
WORKDIR "/src/Listjj/"

RUN /root/.dotnet/tools/dotnet-ef migrations add DockerInitMigrations

RUN chmod +x ./Setup.sh
CMD /bin/bash ./Setup.sh