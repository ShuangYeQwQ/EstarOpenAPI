# Use the ASP.NET runtime image as the base for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8081
EXPOSE 8080

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj files and restore dependencies
COPY ["EstarOpenAPI/EstarOpenAPI.csproj", "EstarOpenAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["FireStoreModel/FireStoreModel.csproj", "FireStoreModel/"]
COPY ["GoogleCloudModel/GoogleCloudModel.csproj", "GoogleCloudModel/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["EStarGoogleCloud/EStarGoogleCloud.csproj", "EStarGoogleCloud/"]

RUN dotnet restore "./EstarOpenAPI/EstarOpenAPI.csproj"

# Copy the remaining source files
COPY . .

# Trust the development certificates (for HTTPS)
RUN dotnet dev-certs https
RUN dotnet dev-certs https --trust

# Build the application
WORKDIR "/src/EstarOpenAPI"
RUN dotnet build "./EstarOpenAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "./EstarOpenAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the base image to create the final runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EstarOpenAPI.dll"]
