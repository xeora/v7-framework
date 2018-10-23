using System.Threading;

namespace Xeora.Web.Service.DSS
{
    public class DSSManager
    {
        private IDSSManager _DSSManager;

        public DSSManager()
        {
            switch (Basics.Configurations.Xeora.DSS.ServiceType)
            {
                case Basics.Configuration.DSSServiceTypes.External:
                    this._DSSManager = 
                        new ExternalManager(
                            Basics.Configurations.Xeora.DSS.ServiceEndPoint
                        );

                    break;
                default:
                    this._DSSManager =
                        new MemoryManager();
                    
                    break;
            }
        }

        private static object _Lock = new object();
        private static DSSManager _Current = null;
        public static IDSSManager Current
        {
            get 
            {
                Monitor.Enter(DSSManager._Lock);
                try
                {
                    if (DSSManager._Current == null)
                        DSSManager._Current = new DSSManager();
                }
                finally
                {
                    Monitor.Exit(DSSManager._Lock);
                }

                return DSSManager._Current._DSSManager;
            }
        }
    }
}
