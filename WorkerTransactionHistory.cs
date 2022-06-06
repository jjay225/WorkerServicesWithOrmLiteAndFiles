using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReplicationTransformCleaner.FileReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReplicationTransformCleaner
{
    public class WorkerTransactionHistory : BackgroundService
    {
        private readonly ILogger<WorkerTransactionHistory> _logger;
        private readonly ITransactionHistoryFileReaderService _transactionHistoryFileReaderService;

        public WorkerTransactionHistory(ILogger<WorkerTransactionHistory> logger, ITransactionHistoryFileReaderService transactionHistoryFileReaderService)
        {
            _logger = logger;
            _transactionHistoryFileReaderService = transactionHistoryFileReaderService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _transactionHistoryFileReaderService.ReadFile();

                await Task.Delay(6000, stoppingToken);
            }
        }
    }
}