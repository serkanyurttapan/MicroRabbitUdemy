using System.Text;
using System.Text.Json.Serialization;
using MediatR;
using MicroRabbit.Domain.Bus;
using MicroRabbit.Domain.Commands;
using MicroRabbit.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus : IEventBus
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
            channel.BasicPublish("", eventName, null, body);
        }
    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType?.Name} already is registered for {eventName ?? ""}",
                nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();
    }

    private void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var eventName = typeof(T).Name;
        channel.QueueDeclare(eventName, false, false, false, null);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += Consumer_Received;
        channel.BasicConsume(eventName, true, consumer);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var messageBody = System.Text.Json.JsonSerializer.Serialize(e);
        await ProcessEvent(eventName, messageBody).ConfigureAwait(false);
    }

    private async Task ProcessEvent(string eventName, string messageBody)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var subsriptions = _handlers[eventName];

            foreach (var subsription in subsriptions)
            {
                var handler = Activator.CreateInstance(subsription);
                if (handler is null)
                    continue;

                var eventType = _eventTypes.FirstOrDefault(t => t.Name == eventName);

                if (eventType is null)
                    continue;

                var @event = System.Text.Json.JsonSerializer.Deserialize(messageBody, eventType);

                if (@event is null)
                    continue;

                var concreateType = typeof(IEventHandler<>).MakeGenericType(eventType);

                await ((Task)concreateType.GetMethod("Handle")?.Invoke(handler, new object?[] { @event })!)!;
            }
        }
    }
}