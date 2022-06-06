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
        //private const string ExecutablePath = @"D:\Code\Personal\Samples\WorkerServicesWithOrmLiteAndFiles\bin\Release\netcoreapp3.1\publish\win-x64\";

        private const string ExecutablePath = @"C:\Program Files (x86)\PayM8\ReplicationTransformCleaner\";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 .WriteTo.File(@"C:\Logs\ReplicationTransformCleaner\LogFile.txt")
                 //.WriteTo.Console()
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
                .UseContentRoot(ExecutablePath)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config
                        .AddJsonFile($"{ExecutablePath}appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"{ExecutablePath}appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerPaymentArrangement>()
                    .AddHostedService<WorkerTransaction>()
                    .AddHostedService<WorkerTransactionHistory>()
                    .AddSingleton<IPaymentArrangementFileReaderService, PaymentArrangementFileReaderService>()
                    .AddSingleton<ITransactionFileReaderService, TransactionFileReaderService>()
                    .AddSingleton<ITransactionHistoryFileReaderService, TransactionHistoryFileReaderService>()
                    .Configure<HostOptions>(
                        opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(15));
                })
               .UseSerilog();
        }
    }
}