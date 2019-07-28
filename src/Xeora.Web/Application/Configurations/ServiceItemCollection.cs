using System;
using System.Collections.Generic;

namespace Xeora.Web.Application.Configurations
{
    public class ServiceItemCollection : List<ServiceItem>, Basics.Domain.IServiceItemCollection
    {
        public Basics.Domain.IServiceItem GetServiceItem(string id)
        {
            foreach (ServiceItem sI in this)
            {
                if (string.Compare(sI.Id, id, StringComparison.OrdinalIgnoreCase) == 0)
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

        public string[] GetAuthenticationKeys() => 
            this.Count > 0 ? base[0].AuthenticationKeys : new string[] { };
    }
}
