using System.Threading;

namespace Xeora.Web.Service.Dss
{
    public class Manager
    {
        private readonly IManager _Manager;

        private Manager()
        {
            switch (Basics.Configurations.Xeora.Dss.ServiceType)
            {
                case Basics.Configuration.DssServiceTypes.External:
                    this._Manager = 
                        new External.Manager(
                            Basics.Configurations.Xeora.Dss.ServiceEndPoint
                        );

                    break;
                default:
                    this._Manager =
                        new Internal.Manager();
                    
                    break;
            }
        }

        private static readonly object Lock = new object();
        private static Manager _Current;
        public static IManager Current
        {
            get 
            {
                Monitor.Enter(Manager.Lock);
                try
                {
                    if (Manager._Current == null)
                        Manager._Current = new Manager();
                }
                finally
                {
                    Monitor.Exit(Manager.Lock);
                }

                return Manager._Current._Manager;
            }
        }
    }
}
