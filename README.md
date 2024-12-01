# controle-pedidos-api-pagamentos


### Execução de migration

```bash
 dotnet ef migrations add {MigrationName} --project adapter/driven/CP.Pedidos.Data -s adapter/driver/CP.Pedidos.Api -c ControlePedidoContext --verbose
 ```
