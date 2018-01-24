using System.Collections.Generic;

namespace Xeora.Web.Site.Setting
{
    public class ServiceItemCollection : List<ServiceItem>, Basics.Domain.IServiceItemCollection
    {
        public Basics.Domain.IServiceItem GetServiceItem(string ID)
        {
            foreach (ServiceItem sI in this)
            {
                if (string.Compare(sI.ID, ID, true) == 0)
                    return sI;
            }

            return null;
        }

        public Basics.Domain.IServiceItemCollection GetServiceItems(Basics.Domain.ServiceTypes serviceType)
        {
            ServiceItemCollection rCollection = new ServiceItemCollection();

            foreach (ServiceItem sI in this.ToArray())
            {
                if (sI.ServiceType == serviceType)
                    rCollection.Add(sI);
            }

            return rCollection;
        }

        public string[] GetAuthenticationKeys()
        {
            if (this.Count > 0)
                return base[0].AuthenticationKeys;

            return new string[] { };
        }
    }
}
