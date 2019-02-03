using System.Threading;

namespace Xeora.Web.Manager
{
    public class Master
    {
        private static object _InitLock = new object();
        private static bool _Initialized = false;

        public static void Initialize()
        {
            Monitor.Enter(Master._InitLock);
            try
            {
                if (Master._Initialized)
                    return;

                Loader.Initialize(Master.ClearCache);

                Master._Initialized = true;
            }
            finally
            {
                Monitor.Exit(Master._InitLock);
            }
        }

        public static void ClearCache()
        {
            Application.Dispose();
            StatementFactory.Dispose();
        }
    }
}
