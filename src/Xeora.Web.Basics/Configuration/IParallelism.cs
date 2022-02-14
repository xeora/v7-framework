namespace Xeora.Web.Basics.Configuration
{
    public interface IParallelism
    {
        ushort MaxConnection { get; }
        ushort Magnitude { get; }
        public ushort WorkerThreads { get; }
    }
}
