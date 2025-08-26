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
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        var paymentToProcess = JsonSerializer.Deserialize<PaymentToProcessDto>(message.Body.ToString());

        await Task.Delay(2000);

        var random = new Random();
        bool sucesso = random.Next(2) == 0;

        var paymentResult = new PaymentResultDto
        {
            PagamentoId = paymentToProcess!.PagamentoId,
            Sucesso = sucesso
        };

        var resultJson = JsonSerializer.Serialize(paymentResult);

        var sender = _serviceBusClient.CreateSender("payment-result");
        var resultMessage = new ServiceBusMessage(resultJson);
        await sender.SendMessageAsync(resultMessage);

        await messageActions.CompleteMessageAsync(message);
    }
}