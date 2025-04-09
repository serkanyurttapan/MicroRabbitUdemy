using System.Reflection;
using System.Runtime.CompilerServices;
using MediatR;
using MicroRabbit.Infra.IoC;
using MicroRabbit.Microservices.Banking.Application.Interfaces;
using MicroRabbit.Microservices.Banking.Application.Models;
using MicroRabbit.Microservices.Banking.Data.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BankingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
});
DependencyContainer.RegisteredServices(builder.Services);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Banking microservice",
        Version = "v1"
    });
});

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Microservice V1"); });

app.UseHttpsRedirection();

app.MapGet("/banking", (IAccountService _accountService) => { return _accountService.GetAccounts(); });

app.MapPost("/banking", (AccountTransfer transfer, IAccountService _accountService) =>
{
    _accountService.Transfer(transfer);
    return Results.Ok(transfer);
});


app.Run();