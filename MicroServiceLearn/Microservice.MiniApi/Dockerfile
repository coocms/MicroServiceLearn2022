#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Microservice.MiniApi/Microservice.MiniApi.csproj", "Microservice.MiniApi/"]
COPY ["Microserivce.Interface/Microserivce.Interface.csproj", "Microserivce.Interface/"]
COPY ["Microservice.Model/Microservice.Model.csproj", "Microservice.Model/"]
COPY ["Microservice.Service/Microservice.Service.csproj", "Microservice.Service/"]
COPY ["Microservice.Framework/Microservice.Framework.csproj", "Microservice.Framework/"]
RUN dotnet restore "Microservice.MiniApi/Microservice.MiniApi.csproj"
COPY . .
WORKDIR "/src/Microservice.MiniApi"
RUN dotnet build "Microservice.MiniApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservice.MiniApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.MiniApi.dll"]