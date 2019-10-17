using System;
using System.Reflection;

namespace Xeora.Web.Manager.Execution
{
    internal class MethodInfoFinder
    {
        private readonly Basics.Context.Request.HttpMethod _HttpMethod;
        private readonly string _SearchName;

        public MethodInfoFinder(Basics.Context.Request.HttpMethod httpMethod, string searchName)
        {
            this._HttpMethod = httpMethod;
            this._SearchName = searchName;

            this.Identifier = $"{this._HttpMethod}_{this._SearchName}";
        }

        public string Identifier { get; }

        public bool Find(MethodInfo mI)
        {
            foreach (object aT in mI.GetCustomAttributes(false))
            {
                Type workingType = aT.GetType();

                if (workingType != typeof(Basics.Attribute.HttpMethodAttribute)) continue;
                
                Basics.Context.Request.HttpMethod httpMethod =
                    (Basics.Context.Request.HttpMethod)workingType.InvokeMember("Method", BindingFlags.GetProperty, null, aT, null);
                object bindProcedureName =
                    workingType.InvokeMember("BindProcedureName", BindingFlags.GetProperty, null, aT, null);
                        
                if (httpMethod != this._HttpMethod) return false;

                if (!string.IsNullOrEmpty(bindProcedureName.ToString()))
                    return string.CompareOrdinal(bindProcedureName.ToString(), this._SearchName) == 0;

                break;
            }

            return string.Compare(this._SearchName, mI.Name, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
