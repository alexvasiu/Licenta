FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /MusicIdentifierAPI

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY MusicIdentifierAPI/*.csproj ./MusicIdentifierAPI/
COPY UnitTesting/*.csproj ./UnitTesting/
COPY Licenta/*.csproj ./Licenta/
COPY PopulateDatabase/*.csproj ./PopulateDatabase/
RUN dotnet restore Licenta.sln

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out MusicIdentifierAPI/MusicIdentifierAPI.csproj

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /MusicIdentifierAPI
COPY --from=build-env MusicIdentifierAPI/MusicIdentifierAPI/out .

CMD dotnet MusicIdentifierAPI.dll --urls "http://*:$PORT"
