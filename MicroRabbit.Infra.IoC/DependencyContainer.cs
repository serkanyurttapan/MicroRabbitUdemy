using MediatR;
using MicroRabbit.Domain.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Microservices.Banking.Application.Interfaces;
using MicroRabbit.Microservices.Banking.Application.Services;
using MicroRabbit.Microservices.Banking.Data.Context;
using MicroRabbit.Microservices.Banking.Data.Repository;
using MicroRabbit.Microservices.Banking.Domain.CommandHandlers;
using MicroRabbit.Microservices.Banking.Domain.Commands;
using MicroRabbit.Microservices.Banking.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MicroRabbit.Infra.IoC;

public class DependencyContainer
{
    public static void RegisteredServices(IServiceCollection services)
    {
        //Domain Bus
        services.AddTransient<IEventBus, RabbitMQBus>();
        
        //Domain Banking Commands
        services.AddTransient<IRequestHandler<CreateTransferCommand,bool>, TransferCommandHandler>();
        
        //Aplication Services
        services.AddTransient<IAccountService, AccountService>();
        
        //Data
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<BankingDbContext>();  
        
        
    }
}