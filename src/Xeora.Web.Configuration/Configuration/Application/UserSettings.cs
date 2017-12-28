using System.Collections.Generic;

namespace Xeora.Web.Configuration
{
    public class UserSettings : Dictionary<string, string>, Basics.Configuration.IUserSettings
    {
        public new string this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                    return base[key];

                return string.Empty;
            }
        }
    }
}
