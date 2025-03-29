using MicroRabbit.Microservices.Banking.Data.Context;
using MicroRabbit.Microservices.Banking.Domain.Interfaces;
using MicroRabbit.Microservices.Banking.Domain.Models;

namespace MicroRabbit.Microservices.Banking.Data.Repository;

public class AccountRepository :IAccountRepository
{
    private readonly BankingDbContext _bankingDbContext;

    public AccountRepository(BankingDbContext bankingDbContext)
    {
        _bankingDbContext = bankingDbContext;
    }
    public IEnumerable<Account> GetAccounts()
    {
        return _bankingDbContext.Accounts;
    }
}