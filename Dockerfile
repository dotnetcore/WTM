FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /app

COPY . .
RUN dotnet publish "./demo/WalkingTec.Mvvm.Demo/WalkingTec.Mvvm.Demo.csproj" -c Release -o /app/out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_URLS http://+:80
ENTRYPOINT ["dotnet", "WalkingTec.Mvvm.Demo.dll"]