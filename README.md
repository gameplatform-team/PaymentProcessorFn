# PaymentProcessorFn

Bem-vindo ao repositório **PaymentProcessorFn**!

Este projeto faz parte da equipe GamePlatform e implementa uma **Azure Function** desenvolvida em **C#** para o processamento de pagamentos, realizando integrações com diferentes gateways de pagamento de forma segura e eficiente.

## Visão Geral

- **Processamento de Pagamentos:** Responsável por orquestrar e executar transações financeiras, integrando-se com múltiplos gateways de pagamento.
- **Azure Function:** Implementação baseada em funções serverless, garantindo escalabilidade e alta disponibilidade.
- **Azure Service Bus:** O gatilho da função é a publicação de mensagens em uma fila do Azure Service Bus. Cada mensagem representa uma solicitação de processamento de pagamento.

## Tecnologias Utilizadas

- **C#** (linguagem principal)
- **Azure Functions**
- **Azure Service Bus**
