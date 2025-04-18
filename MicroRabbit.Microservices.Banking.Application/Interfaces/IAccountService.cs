using MicroRabbit.Microservices.Banking.Application.Models;
using MicroRabbit.Microservices.Banking.Domain.Models;

namespace MicroRabbit.Microservices.Banking.Application.Interfaces;

public interface IAccountService
{
    IEnumerable<Account> GetAccounts();
    void Transfer(AccountTransfer transfer);
}