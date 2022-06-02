﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicationTransformCleaner.FileReaders
{
    public class TransactionHistoryFileReaderService : ITransactionHistoryFileReaderService
    {
        private readonly ILogger<TransactionHistoryFileReaderService> _logger;
        private readonly IConfiguration _config;
        private readonly string _errorDirectory;
        private readonly string _errFileExt;

        public TransactionHistoryFileReaderService(ILogger<TransactionHistoryFileReaderService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _errorDirectory = _config.GetValue<string>("ErrorDirectory");
            _errFileExt = _config.GetValue<string>("ErrorExtension");
        }

        public void ReadFile()
        {
            var errorFiles = Directory.EnumerateFiles(_errorDirectory, _errFileExt, SearchOption.TopDirectoryOnly)
            .Where(f => Path.GetFileName(f).StartsWith("TxHistoryErrors"));

            if (!errorFiles.Any())
            {
                _logger.LogWarning("No files found!");
            }

            foreach (var errorFile in errorFiles)
            {
                var fileContent = File.ReadLines(errorFile);
                _logger.LogInformation("File name, {fileName}", Path.GetFileName(errorFile));

                foreach (var fileLine in fileContent)
                {
                    var dataLen = fileLine.IndexOf(",");
                    var txId = fileLine[..dataLen];
                    _logger.LogInformation("TxId for History {TxId}", txId);
                }
            }
        }

        public void PingPong()
        {
            _logger.LogDebug("Ping! Pong from Transaction History!!");
        }
    }
}