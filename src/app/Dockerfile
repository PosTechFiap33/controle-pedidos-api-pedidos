
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ControlePedido.Pedidos/adapter/driver/CP.Pedidos.Api/CP.Pedidos.Api.csproj"
COPY . .
WORKDIR "/src/ControlePedido.Pedidos/adapter/driver/CP.Pedidos.Api"
RUN dotnet build "CP.Pedidos.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CP.Pedidos.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CP.Pedidos.Api.dll"]
