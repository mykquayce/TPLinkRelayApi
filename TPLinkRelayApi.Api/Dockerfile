FROM mcr.microsoft.com/dotnet/sdk:8.0 as build-env

RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates

WORKDIR /app
COPY . .
RUN dotnet restore ./TPLinkRelayApi.Api/TPLinkRelayApi.Api.csproj --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet publish ./TPLinkRelayApi.Api/TPLinkRelayApi.Api.csproj --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "./TPLinkRelayApi.Api.dll"]
