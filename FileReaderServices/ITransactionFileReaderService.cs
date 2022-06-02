namespace ReplicationTransformCleaner.FileReaders
{
    public interface ITransactionFileReaderService
    {
        void ReadFile();

        void PingPong();
    }
}