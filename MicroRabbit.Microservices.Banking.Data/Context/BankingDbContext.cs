using System.Security.Cryptography.X509Certificates;
using MicroRabbit.Microservices.Banking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Microservices.Banking.Data.Context;

public class BankingDbContext :DbContext
{
   public BankingDbContext(DbContextOptions options):base(options)
   {
   }
   public DbSet<Account> Accounts { get; set; }

}