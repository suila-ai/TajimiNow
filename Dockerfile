FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /workspace
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0

ENV TZ=Asia/Tokyo

WORKDIR /app
COPY --from=build /workspace/out .

ENTRYPOINT [ "dotnet", "TajimiNow.dll" ]