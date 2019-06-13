FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /app

COPY . .
RUN dotnet publish "./doc/WalkingTec.Mvvm.Doc/WalkingTec.Mvvm.Doc.csproj" -c Release -o /app/out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_URLS http://+:80
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "WalkingTec.Mvvm.Doc.dll"]