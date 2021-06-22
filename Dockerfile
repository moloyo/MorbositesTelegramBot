#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
COPY . .
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
#WORKDIR /src
#COPY ["MorbositesBotApi.csproj", "."]
#RUN dotnet restore "./MorbositesBotApi.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "MorbositesBotApi.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "MorbositesBotApi.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "MorbositesBotApi.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet MorbositesBotApi.dll