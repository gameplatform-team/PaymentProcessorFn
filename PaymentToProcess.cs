using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Gameplatform.Function.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gameplatform.Function;

public class PaymentToProcess
{
    private readonly ILogger<PaymentToProcess> _logger;
    private readonly ServiceBusClient _serviceBusClient;

    public PaymentToProcess(ILogger<PaymentToProcess> logger, IConfiguration configuration)
    {
        _logger = logger;

        var serviceBusConnectionString = configuration["Values:gameplatform_SERVICEBUS"];
        _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
    }

    [Function(nameof(PaymentToProcess))]
    public async Task Run(
        [ServiceBusTrigger("payment-to-process", Connection = "gameplatform_SERVICEBUS")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message Body: {body}", message.Body);

        var paymentToProcess = JsonSerializer.Deserialize<PaymentToProcessDto>(message.Body.ToString());

        await Task.Delay(TimeSpan.FromSeconds(5));

        var random = new Random();
        bool sucesso = random.Next(3) != 0;

        var paymentResult = new PaymentResultDto
        {
            PagamentoId = paymentToProcess!.pagamentoId,
            Sucesso = sucesso
        };

        var resultJson = JsonSerializer.Serialize(paymentResult);

        var sender = _serviceBusClient.CreateSender("payment-result");
        var resultMessage = new ServiceBusMessage(resultJson);
        await sender.SendMessageAsync(resultMessage);

        _logger.LogInformation("Result: {resultJson}", resultJson);

        await messageActions.CompleteMessageAsync(message);
    }
}