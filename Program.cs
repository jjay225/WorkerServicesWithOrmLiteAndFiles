using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReplicationTransformCleaner.FileReaders;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace ReplicationTransformCleaner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 //.WriteTo.File(@"C:\temp\workerservice\LogFile.txt")
                 .WriteTo.Console()
                 .CreateLogger();

            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the serivce");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerTransactionHistory>()
                    .AddHostedService<WorkerTransaction>()
                    .AddHostedService<WorkerPaymentArrangement>()
                    .AddSingleton<ITransactionHistoryFileReaderService, TransactionHistoryFileReaderService>()
                    .AddSingleton<ITransactionFileReaderService, TransactionFileReaderService>()
                    .AddSingleton<IPaymentArrangementFileReaderService, PaymentArrangementFileReaderService>();
                })
                .UseSerilog();
        }
    }
}