namespace ReplicationTransformCleaner.FileReaders
{
    public interface IPaymentArrangementFileReaderService
    {
        void ReadFile();

        void PingPong();
    }
}