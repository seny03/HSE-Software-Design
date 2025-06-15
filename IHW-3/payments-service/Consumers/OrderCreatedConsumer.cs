using MassTransit;
using PaymentsService.Contracts;
using PaymentsService.Services;

namespace PaymentsService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly IPaymentsService _paymentsService;
    private readonly ITopicProducer<Guid, PaymentCompleted> _paymentCompletedProducer;
    private readonly ITopicProducer<Guid, PaymentFailed> _paymentFailedProducer;

    public OrderCreatedConsumer(
        IPaymentsService paymentsService,
        ITopicProducer<Guid, PaymentCompleted> paymentCompletedProducer,
        ITopicProducer<Guid, PaymentFailed> paymentFailedProducer)
    {
        _paymentsService = paymentsService;
        _paymentCompletedProducer = paymentCompletedProducer;
        _paymentFailedProducer = paymentFailedProducer;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var order = context.Message;
        var (paymentSuccessful, _) = await _paymentsService.WithdrawAsync(order.UserId, order.TotalAmount);

        if (paymentSuccessful)
        {
            await _paymentCompletedProducer.Produce(order.OrderId, new PaymentCompleted { OrderId = order.OrderId });
        }
        else
        {
            await _paymentFailedProducer.Produce(order.OrderId, new PaymentFailed { OrderId = order.OrderId });
        }
    }
} 