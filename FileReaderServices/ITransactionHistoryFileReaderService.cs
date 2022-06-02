namespace ReplicationTransformCleaner.FileReaders
{
    public interface ITransactionHistoryFileReaderService
    {
        void ReadFile();

        void PingPong();
    }
}