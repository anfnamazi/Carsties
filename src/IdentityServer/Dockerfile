FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY Carsties.sln ./
COPY src/IdentityServer/IdentityServer.csproj ./src/IdentityServer/

RUN dotnet restore ./src/IdentityServer/IdentityServer.csproj

COPY . .
RUN dotnet publish ./src/IdentityServer/IdentityServer.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "IdentityServer.dll" ]