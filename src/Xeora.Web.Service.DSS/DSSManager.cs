using System.Threading;

namespace Xeora.Web.Service.Dss
{
    public class DssManager
    {
        private IDssManager _DssManager;

        public DssManager()
        {
            switch (Basics.Configurations.Xeora.Dss.ServiceType)
            {
                case Basics.Configuration.DssServiceTypes.External:
                    this._DssManager = 
                        new ExternalManager(
                            Basics.Configurations.Xeora.Dss.ServiceEndPoint
                        );

                    break;
                default:
                    this._DssManager =
                        new MemoryManager();
                    
                    break;
            }
        }

        private static object _Lock = new object();
        private static DssManager _Current = null;
        public static IDssManager Current
        {
            get 
            {
                Monitor.Enter(DssManager._Lock);
                try
                {
                    if (DssManager._Current == null)
                        DssManager._Current = new DssManager();
                }
                finally
                {
                    Monitor.Exit(DssManager._Lock);
                }

                return DssManager._Current._DssManager;
            }
        }
    }
}
