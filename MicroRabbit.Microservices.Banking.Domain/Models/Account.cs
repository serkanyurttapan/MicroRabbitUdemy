namespace MicroRabbit.Microservices.Banking.Domain.Models;

public class Account
{
    public int Id { get; set; }
    public string AccountType { get; set; }
    public string AccountBalance { get; set; }
}