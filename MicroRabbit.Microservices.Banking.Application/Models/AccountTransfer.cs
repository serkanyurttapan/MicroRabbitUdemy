namespace MicroRabbit.Microservices.Banking.Application.Models;

public class AccountTransfer
{
    public int FromAccount { get; set; }
    public int ToAccount { get; set; }
    public decimal TransferAmount { get; set; }
}