FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

RUN apk add --no-cache clang build-base zlib-dev

WORKDIR /src

COPY ["RinhaBackend.csproj", "./"]
RUN dotnet restore "RinhaBackend.csproj"

COPY . .

RUN dotnet publish "RinhaBackend.csproj" -c Release -r linux-musl-x64 -o /app/publish /p:PublishAot=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS final

WORKDIR /app
EXPOSE 8080

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

COPY Data/references.bin ./Data/references.bin

ENTRYPOINT ["./RinhaBackend"]