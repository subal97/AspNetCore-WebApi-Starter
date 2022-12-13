#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_cache
WORKDIR /src
COPY DemoREST.csproj DemoREST/
RUN dotnet restore DemoREST/DemoREST.csproj

FROM build_cache as build
COPY . /src/DemoREST/
WORKDIR /src/DemoREST
RUN dotnet publish DemoREST.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DemoREST.dll", "--launch-profile Production"]
