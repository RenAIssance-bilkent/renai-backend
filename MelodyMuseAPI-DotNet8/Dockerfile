#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Set the working directory and copy the necessary files
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MelodyMuseAPI-DotNet8/MelodyMuseAPI.csproj", "MelodyMuseAPI-DotNet8/"]
RUN dotnet restore "MelodyMuseAPI-DotNet8/MelodyMuseAPI.csproj"
COPY . .
WORKDIR "/src/MelodyMuseAPI-DotNet8"
RUN dotnet build "MelodyMuseAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MelodyMuseAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MelodyMuseAPI.dll"]
