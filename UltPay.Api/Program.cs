using Microsoft.EntityFrameworkCore;
using UltPay.Infrastructure.Persistence;
using UltPay.Api.Services;
using UltPay.Infrastructure.Providers.Flutterwave;
using UltPay.Api.Providers;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UltPayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransferExecutionService, TransferExecutionService>();

builder.Services.Configure<FlutterwaveOptions>(
    builder.Configuration.GetSection("Flutterwave"));

builder.Services.AddHttpClient<FlutterwaveTransferProvider>();

builder.Services.AddScoped<ITransferProvider, FlutterwaveTransferProvider>();
builder.Services.AddScoped<ITransferProviderResolver, TransferProviderResolver>();

var app = builder.Build();

app.MapControllers();

app.Run();