using System.Threading;

namespace Xeora.Web.Manager
{
    public static class Master
    {
        private static readonly object Lock = new object();
        private static bool _Initialized;

        public static void Initialize()
        {
            Monitor.Enter(Master.Lock);
            try
            {
                if (Master._Initialized)
                    return;

                Loader.Initialize();

                Master._Initialized = true;
            }
            finally
            {
                Monitor.Exit(Master.Lock);
            }
        }

        public static void Reset()
        {
            Monitor.Enter(Master.Lock);
            try
            {
                if (!Master._Initialized)
                    return;
                
                Loader.Reload();
            }
            finally
            {
                Monitor.Exit(Master.Lock);
            }
        }
    }
}
