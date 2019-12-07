namespace Xeora.Web.Basics.Configuration
{
    public interface IParallelism
    {
        short Worker { get; }
        short WorkerThread { get; }
        short Bucket { get; }
        short BucketThread { get; }
    }
}
