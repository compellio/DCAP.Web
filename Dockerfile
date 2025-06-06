# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

COPY . /source
WORKDIR /source/Compellio.DCAP.Web

ARG TARGETARCH
ARG CONFIGURATION=Debug

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} -c ${CONFIGURATION} --use-current-runtime --self-contained false -o /app 

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "Compellio.DCAP.Web.dll"]