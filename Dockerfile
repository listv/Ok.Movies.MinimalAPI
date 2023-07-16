FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ./Ok.Movies.sln .

COPY ./src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./src/${file%.*}/ && mv $file ./src/${file%.*}/; done

COPY ./tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./tests/${file%.*}/ && mv $file ./tests/${file%.*}/; done

RUN dotnet restore

COPY . .
RUN dotnet build -c Release

FROM build AS publish
RUN dotnet publish "/src/Api/Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
