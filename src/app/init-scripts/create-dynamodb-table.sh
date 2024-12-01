#!/bin/bash

# Espera o LocalStack iniciar e estar pronto
echo "Aguardando o LocalStack iniciar..."
until awslocal dynamodb list-tables; do
    sleep 5
done

echo "LocalStack iniciado. Criando a tabela DynamoDB..."

# Cria a tabela no DynamoDB
awslocal dynamodb create-table  --table-name ControlePedidosPagamentos  --attribute-definitions AttributeName=PedidoId,AttributeType=S  --key-schema AttributeName=PedidoId,KeyType=HASH --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5

# Lista dados do dynamodb
awslocal dynamodb scan --table-name ControlePedidosPagamentos

# Cria a queue no SQS
awslocal sqs create-queue --queue-name ControlePedidosPagamentos --attributes MessageRetentionPeriod=345600

# Listar mensagens no SQS
awslocal sqs receive-message --queue-url http://localhost:4566/000000000000/ControlePedidosPagamentos --max-number-of-messages 10

echo "Tabela criada com sucesso!"
