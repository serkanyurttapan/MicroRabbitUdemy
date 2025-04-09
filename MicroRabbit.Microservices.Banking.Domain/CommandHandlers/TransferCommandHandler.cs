using MediatR;
using MicroRabbit.Domain.Bus;
using MicroRabbit.Microservices.Banking.Domain.Commands;
using MicroRabbit.Microservices.Banking.Domain.Events;

namespace MicroRabbit.Microservices.Banking.Domain.CommandHandlers;

public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
{
    private readonly IEventBus _bus;

    public TransferCommandHandler(IEventBus bus)
    {
        _bus = bus;
    }

    public Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
       _bus.Publish(new TransferCreatedEvent(request.From, request.To, request.Amount));
        return Task.FromResult(true);
    }
}