using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItemCollection : List<ResolveItem>
    {
        public ResolveItem this[string id]
        {
            get
            {
                foreach (ResolveItem resolveItem in this)
                {
                    if (string.Compare(resolveItem.Id, id, StringComparison.OrdinalIgnoreCase) == 0)
                        return resolveItem;
                }

                return new ResolveItem(id);
            }
        }
    }
}
