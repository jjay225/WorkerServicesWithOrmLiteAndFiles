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
    public class WorkerPaymentArrangement : BackgroundService
    {
        private readonly ILogger<WorkerPaymentArrangement> _logger;
        private readonly IPaymentArrangementFileReaderService _paymentarrangementFileReaderService;

        public WorkerPaymentArrangement(ILogger<WorkerPaymentArrangement> logger, IPaymentArrangementFileReaderService paymentArrangementFileReaderService)
        {
            _logger = logger;
            _paymentarrangementFileReaderService = paymentArrangementFileReaderService;
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
                _logger.LogInformation("WorkerTransaction running at: {time}", DateTimeOffset.Now);
                _paymentarrangementFileReaderService.ReadFile();

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}