using Microsoft.EntityFrameworkCore;
using UltPay.Infrastructure.Persistence;
using UltPay.Api.Services;
using UltPay.Api.BackgroundServices;
using UltPay.Api.Providers;

using UltPay.Infrastructure.Providers.Flutterwave;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ITransferExecutionService, TransferExecutionService>();
builder.Services.AddControllers();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddDbContext<UltPayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));
builder.Services.Configure<FlutterwaveOptions>(
    builder.Configuration.GetSection("Flutterwave"));

builder.Services.AddHttpClient<FlutterwaveTransferProvider, FlutterwaveTransferProvider>();


builder.Services.AddScoped<ITransferProviderResolver, TransferProviderResolver>();

//builder.Services.AddHostedService<TransferProcessorService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransferExecutionService, TransferExecutionService>();

builder.Services.AddScoped<ITransferProvider, FlutterwaveTransferProvider>();
builder.Services.AddScoped<ITransferProviderResolver, TransferProviderResolver>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<UltPayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
