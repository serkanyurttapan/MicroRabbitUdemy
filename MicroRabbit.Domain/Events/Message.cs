using MediatR;

namespace MicroRabbit.Domain.Events;

public abstract class Message :IRequest
{
    public string MessageType { get; protected set; }   
    
    protected Message ()
    {
        MessageType = GetType().Name;
    }   
}