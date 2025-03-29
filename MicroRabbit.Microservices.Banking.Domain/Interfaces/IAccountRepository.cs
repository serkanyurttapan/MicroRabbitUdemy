using MicroRabbit.Microservices.Banking.Domain.Models;

namespace MicroRabbit.Microservices.Banking.Domain.Interfaces;

public interface IAccountRepository
{
    IEnumerable<Account> GetAccounts();
}