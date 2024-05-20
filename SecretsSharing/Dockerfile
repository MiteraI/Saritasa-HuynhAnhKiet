#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
ENV DATABASE_URL=""
ENV AWS_ACCESS_KEY=""
ENV AWS_SECRET_KEY=""
ENV BUCKET_NAME=""
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SecretsSharing/SecretsSharing.csproj", "SecretsSharing/"]
COPY ["SecretsSharing.Service/SecretsSharing.Service.csproj", "SecretsSharing.Service/"]
COPY ["SecretsSharing.Dto/SecretsSharing.Dto.csproj", "SecretsSharing.Dto/"]
COPY ["SecretsSharing.Domain/SecretsSharing.Domain.csproj", "SecretsSharing.Domain/"]
COPY ["SecretsSharing.Repository/SecretsSharing.Repository.csproj", "SecretsSharing.Repository/"]
RUN dotnet restore "./SecretsSharing/./SecretsSharing.csproj"
COPY . .
WORKDIR "/src/SecretsSharing"
RUN dotnet build "./SecretsSharing.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SecretsSharing.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecretsSharing.dll"]