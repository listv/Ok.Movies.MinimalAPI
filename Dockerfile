FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY ./Ok.Movies.MinimalAPI.sln .

COPY ./src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./src/${file%.*}/ && mv $file ./src/${file%.*}/; done

COPY ./tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./tests/${file%.*}/ && mv $file ./tests/${file%.*}/; done

COPY ./helpers/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./helpers/${file%.*}/ && mv $file ./helpers/${file%.*}/; done

RUN dotnet restore

COPY . .
RUN dotnet build -c Release --no-restore

FROM build AS publish
RUN dotnet publish "/src/Api/Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ok.Movies.MinimalAPI.Api.dll"]
