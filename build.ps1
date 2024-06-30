docker pull mcr.microsoft.com/dotnet/aspnet:8.0
docker pull mcr.microsoft.com/dotnet/sdk:8.0

docker build `
	--file '.\TPLinkRelayApi.Api\Dockerfile' `
	--secret "id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt" `
	--tag 'eassbhhtgu/tplinkrelayapi:latest' `
	.
