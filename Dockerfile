# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /serviceauthentication

EXPOSE 96
EXPOSE 5201

COPY ./*.csproj ./
RUN dotnet restore 

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0 
WORKDIR /serviceauthentication
COPY --from=build /serviceauthentication/out .
ENTRYPOINT ["dotnet", "autenticacion.dll"]
