FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /app

COPY . .
RUN dotnet publish "./demo/WalkingTec.Mvvm.Demo/WalkingTec.Mvvm.Demo.csproj" -c Release -o /app/out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# install libgdiplus for System.Drawing
RUN apk add libgdiplus --update-cache --repository http://dl-cdn.alpinelinux.org/alpine/edge/testing/ --allow-untrusted && \
    apk add terminus-font

ENV ASPNETCORE_URLS http://+:80
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "WalkingTec.Mvvm.Demo.dll"]
