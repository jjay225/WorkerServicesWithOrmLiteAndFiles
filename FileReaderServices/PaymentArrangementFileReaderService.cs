using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReplicationTransformCleaner.OrmLiteTables;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicationTransformCleaner.FileReaders
{
    public class PaymentArrangementFileReaderService : IPaymentArrangementFileReaderService
    {
        private readonly ILogger<PaymentArrangementFileReaderService> _logger;
        private readonly IConfiguration _config;
        private readonly string _errorDirectory;
        private readonly string _errFileExt;
        private readonly string _errorArchiveDirectory;
        private readonly IDbConnection _db;

        public PaymentArrangementFileReaderService(ILogger<PaymentArrangementFileReaderService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _errorDirectory = _config.GetValue<string>("ErrorDirectory");
            _errFileExt = _config.GetValue<string>("ErrorExtension");
            _errorArchiveDirectory = _config.GetValue<string>("ArchiveDirectory");

            var connString = _config.GetConnectionString("SQLServer");
            var dbFactory = new OrmLiteConnectionFactory(connString, SqlServer2019Dialect.Provider);
            _db = dbFactory.OpenDbConnection();
        }

        public void ReadFile()
        {
            var errorFiles = Directory.EnumerateFiles(_errorDirectory, _errFileExt, SearchOption.TopDirectoryOnly)
            .Where(f => Path.GetFileName(f).StartsWith("PAErrors"));

            if (!errorFiles.Any())
            {
                _logger.LogWarning("PaymentArrangement File Reader: No files found!");
            }

            foreach (var errorFile in errorFiles)
            {
                var fileContent = File.ReadLines(errorFile);
                _logger.LogInformation("File name, {fileName}", errorFile);

                foreach (var fileLine in fileContent)
                {
                    if (String.IsNullOrEmpty(fileLine))
                    {
                        break;
                    }

                    var dataLen = fileLine.IndexOf(",");
                    var paId = fileLine[..dataLen];
                    _logger.LogInformation("PaymentArrangementId: {PaId}", paId);
                    var paErrors = new PaErrorsForInsert
                    {
                        PaymentArrangementId = Guid.Parse(paId),
                        HasBeenTransformed = false
                    };

                    try
                    {
                        _db.Insert(paErrors);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("PaymentArrangementId Insert Error!: {PaId}, {PaError}", paId, ex.Message);
                    }
                }

                fileContent = null;

                File.Move(errorFile, _errorArchiveDirectory + Path.GetFileName(errorFile), true);
            }
        }

        public void PingPong()
        {
            _logger.LogDebug("Ping! Pong from Transaction History!!");
        }
    }
}