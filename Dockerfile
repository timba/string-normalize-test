FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /build
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final-debian
COPY --from=build /app .
RUN echo "Run test in Debian image"
RUN dotnet normalize-test.dll

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final-alpine-fixed
RUN apk update && apk add icu
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build /app .
RUN echo "Run test in Alpine fixed image"
RUN dotnet normalize-test.dll

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final-alpine-broken
COPY --from=build /app .
RUN echo "Run test in Alpine original image"
RUN dotnet normalize-test.dll

