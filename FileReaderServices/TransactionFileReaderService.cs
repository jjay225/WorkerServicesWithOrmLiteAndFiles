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
    public class TransactionFileReaderService : ITransactionFileReaderService
    {
        private readonly ILogger<TransactionFileReaderService> _logger;
        private readonly IConfiguration _config;
        private readonly string _errorDirectory;
        private readonly string _errFileExt;
        private readonly string _errorArchiveDirectory;
        private readonly IDbConnection _db;

        public TransactionFileReaderService(ILogger<TransactionFileReaderService> logger, IConfiguration config)
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
            .Where(f => Path.GetFileName(f).StartsWith("TxError"));

            if (!errorFiles.Any())
            {
                _logger.LogWarning("Transaction File Reader: No files found!");
            }

            foreach (var errorFile in errorFiles)
            {
                var fileContent = File.ReadLines(errorFile);
                _logger.LogInformation("Transaction File name, {fileName}", Path.GetFileName(errorFile));

                foreach (var fileLine in fileContent)
                {
                    if (String.IsNullOrEmpty(fileLine))
                    {
                        break;
                    }

                    var dataLen = fileLine.IndexOf(",");
                    var txId = fileLine[..dataLen];
                    _logger.LogInformation("Transaction {TxId}", txId);

                    var txErrors = new TxErrorsForInsert
                    {
                        TransactionId = Guid.Parse(txId),
                        HasBeenTransformed = false
                    };

                    try
                    {
                        _db.Insert(txErrors);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Transaction Insert Error!: {txId}, {TxError}", txId, ex.Message);
                    }
                }

                fileContent = null;

                File.Move(errorFile, _errorArchiveDirectory + Path.GetFileName(errorFile), true);
            }
        }

        public void PingPong()
        {
            _logger.LogDebug("Ping! Pong from Transaction!!");
        }
    }
}