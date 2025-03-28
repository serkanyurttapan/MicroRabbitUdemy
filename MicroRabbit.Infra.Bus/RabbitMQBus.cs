
using MediatR;
using MicroRabbit.Domain.Bus;
using MicroRabbit.Domain.Commands;
using MicroRabbit.Domain.Events;
using RabbitMQ.Client;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus :IEventBus
{
    private readonly IMediator _mediator;
    private Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;
    
    public RabbitMQBus(IMediator mediator)
    {
        _mediator = mediator;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }
    
    public Task SendCommand<T>(T command) where T : Command
    {
       return _mediator.Send(command);
    }

    public async Task Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };
        using var connection = factory.CreateConnection();
        using (var channel = connection.CreateModel())
        {
            var eventName = @event.GetType().Name;
            channel.QueueDeclare(eventName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var message = System.Text.Json.JsonSerializer.Serialize(@event);
            var body = System.Text.Encoding.UTF8.GetBytes(message); 
            channel.BasicPublish("",eventName,null, body);
        }
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        throw new NotImplementedException();
    }
}