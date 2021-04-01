#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["KudVenkat_LINQ/KudVenkat_LINQ.csproj", "KudVenkat_LINQ/"]
RUN dotnet restore "KudVenkat_LINQ/KudVenkat_LINQ.csproj"
COPY . .
WORKDIR "/src/KudVenkat_LINQ"
RUN dotnet build "KudVenkat_LINQ.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KudVenkat_LINQ.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KudVenkat_LINQ.dll"]