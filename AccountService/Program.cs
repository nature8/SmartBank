using AccountService.Data;
using Microsoft.EntityFrameworkCore;
using AccountService.Interfaces;
using AccountService.Repositories;
using AccountService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<AccountTransactionConsumer>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService.Services.AccountService>();
builder.Services.AddSingleton<AccountPublisher>();
builder.Services.AddSingleton<AccountTransactionPublisher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();
app.Run();