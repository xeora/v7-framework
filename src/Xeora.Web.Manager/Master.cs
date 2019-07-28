using System.Threading;

namespace Xeora.Web.Manager
{
    public class Master
    {
        private static readonly object InitLock = new object();
        private static bool _Initialized;

        public static void Initialize()
        {
            Monitor.Enter(Master.InitLock);
            try
            {
                if (Master._Initialized)
                    return;

                Loader.Initialize(Master.ClearCache);

                Master._Initialized = true;
            }
            finally
            {
                Monitor.Exit(Master.InitLock);
            }
        }

        public static void ClearCache()
        {
            Application.Dispose();
            StatementFactory.Dispose();
        }
    }
}
