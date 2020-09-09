using System.Threading;

namespace Xeora.Web.Service.Dss
{
    public class Manager
    {
        private readonly IManager _Manager;

        private Manager()
        {
            this._Manager = Basics.Configurations.Xeora.Dss.ServiceType switch
            {
                Basics.Configuration.DssServiceTypes.External => 
                    new External.Manager(Basics.Configurations.Xeora.Dss.ServiceEndPoint),
                _ => new Internal.Manager()
            };
        }

        private static readonly object Lock = new object();
        private static Manager _current;
        public static IManager Current
        {
            get 
            {
                Monitor.Enter(Manager.Lock);
                try
                {
                    Manager._current ??= new Manager();
                    return Manager._current._Manager;
                }
                finally
                {
                    Monitor.Exit(Manager.Lock);
                }
            }
        }
    }
}
