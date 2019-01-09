using System;
using System.Reflection;

namespace Xeora.Web.Manager
{
    internal class MethodInfoFinder
    {
        private Basics.Context.HttpMethod _HttpMethod;
        private string _SearchName;

        public MethodInfoFinder(Basics.Context.HttpMethod httpMethod, string searchName)
        {
            this._HttpMethod = httpMethod;
            this._SearchName = searchName;

            this.Identifier = string.Format("{0}_{1}", this._HttpMethod, this._SearchName);
        }

        public string Identifier { get; }

        public bool Find(MethodInfo mI)
        {
            foreach (object aT in mI.GetCustomAttributes(false))
            {
                Type workingType = aT.GetType();

                if (workingType == typeof(Basics.Attribute.HttpMethodAttribute))
                {
                    Basics.Context.HttpMethod httpMethod =
                        (Basics.Context.HttpMethod)workingType.InvokeMember("Method", BindingFlags.GetProperty, null, aT, null);
                    object bindProcedureName =
                        workingType.InvokeMember("BindProcedureName", BindingFlags.GetProperty, null, aT, null);
                        
                    if (httpMethod != this._HttpMethod) return false;

                    if (!string.IsNullOrEmpty(bindProcedureName.ToString()))
                        return string.Compare(bindProcedureName.ToString(), this._SearchName) == 0;

                    break;
                }
            }

            return string.Compare(this._SearchName, mI.Name, true) == 0;
        }
    }
}
