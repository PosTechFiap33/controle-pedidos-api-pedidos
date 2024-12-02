Feature: Pedido
  Scenario: Deve cadastrar um pedido vinculado ao cliente com sucesso
    Given que eu iforme o id do cliente "eb222a9d-e491-4f44-b9e7-a070ef7a4f12"
    And que eu adicione o produto de valor 30
    And que eu adicione o produto de valor 20
    When eu fizer uma requisicao para gerar o pedido
    Then o status code deve ser 201
    And os dados do pedido estejam validos
    And o id vinculado no pedido deve ser "eb222a9d-e491-4f44-b9e7-a070ef7a4f12"
    And o valor do pedido deve ser 50
    And o status do pedido deve ser "Criado"
    And os dados do pagamento devem estar vazios

  Scenario: Deve cadastrar um pedidos sem vincular cliente com sucesso
    Given que eu adicione o produto de valor 250
    When eu fizer uma requisicao para gerar o pedido
    Then o status code deve ser 201
    And os dados do pedido estejam validos
    And o id vinculado no pedido deve ser "Cliente não informado"
    And o valor do pedido deve ser 250
    And o status do pedido deve ser "Criado"
    And os dados do pagamento devem estar vazios

  Scenario: Deve exibir erro quando iniciar o preparado do pedido sem pagar
    Given que eu adicione o produto de valor 50
    When eu fizer uma requisicao para gerar o pedido
    And eu fizer uma requisicao para iniciar o preparo
    Then deve ser exibida a mensagem de erro "Não foi realizado o pagamento para o pedido informado!"

  Scenario: Deve iniciar o preparado do pedido quando o pagamento for realizado
     Given que eu ja tenha um pedido pago
     When eu fizer uma requisicao para iniciar o preparo
     Then o status do pedido deve ser "Em preparacao"
     And os dados do pagamento devem estar preenchidos

  Scenario: Deve exibir erro quando finalizar o preparado do pedido sem ter iniciado
     Given que eu ja tenha um pedido pago
    When eu fizer uma requisicao para finalizar o prepado do pedido
    Then deve ser exibida a mensagem de erro "Não foi possível finalizar o preparo do pedido pois o preparo não foi iniciado!"

  Scenario: Deve finalizar o preparado do pedido quando o pedido já foi iniciado
     Given que eu ja tenha um pedido iniciado
     When eu fizer uma requisicao para finalizar o prepado do pedido
     Then o id vinculado no pedido deve ser "Cliente não informado"
     And o status do pedido deve ser "Pronto"

  Scenario: Deve exibir erro quando for feita a entrega do pedido sem finalizar o preparo
    Given que eu adicione o produto de valor 50
    When eu fizer uma requisicao para gerar o pedido
    And eu fizer uma requisicao para realizar a entrega do pedido
    Then deve ser exibida a mensagem de erro "Não foi possível finalizar o pedido pois o preparo não foi finalizado!"

  Scenario: Deve realizar a entrega do pedido quando o pedido já foi finalizado
     Given que eu ja tenha um pedido finalizado
     When eu fizer uma requisicao para realizar a entrega do pedido
     Then o id vinculado no pedido deve ser "Cliente não informado"
     And o status do pedido deve ser "Finalizado"

  Scenario: Deve listar os pedidos
     Given que eu tenha pedidos cadastrados
     When eu fizer uma requisicao listar os pedidos
     Then deve ser exibida a lista dos pedidos