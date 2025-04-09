using MicroRabbit.Domain.Bus;
using MicroRabbit.Microservices.Banking.Application.Interfaces;
using MicroRabbit.Microservices.Banking.Application.Models;
using MicroRabbit.Microservices.Banking.Domain.Commands;
using MicroRabbit.Microservices.Banking.Domain.Interfaces;
using MicroRabbit.Microservices.Banking.Domain.Models;

namespace MicroRabbit.Microservices.Banking.Application.Services;

public class AccountService :IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEventBus _eventBus;

    public AccountService(IAccountRepository accountRepository,IEventBus eventBus)
    {
        _accountRepository = accountRepository;
        _eventBus = eventBus;
    }
    public IEnumerable<Account> GetAccounts()
    {
        return _accountRepository.GetAccounts();
    }

    public void Transfer(AccountTransfer transfer)
    {
        var transferCommand = new CreateTransferCommand(transfer.FromAccount,transfer.ToAccount,transfer.TransferAmount);
          
        Task.FromResult(_eventBus.SendCommand(transferCommand));
    }
}