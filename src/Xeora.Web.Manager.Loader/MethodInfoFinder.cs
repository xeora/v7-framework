using System;
using System.Reflection;

namespace Xeora.Web.Manager
{
    internal class MethodInfoFinder
    {
        private string _HttpMethodType;
        private string _SearchName;

        public MethodInfoFinder(string httpMethodType, string searchName)
        {
            this._HttpMethodType = httpMethodType;
            this._SearchName = searchName;

            this.Identifier = string.Format("{0}_{1}", this._HttpMethodType, this._SearchName);
        }

        public string Identifier { get; }

        public bool Find(MethodInfo mI)
        {
            bool attributeCheck = (string.Compare(this._SearchName, mI.Name, true) == 0);

            foreach (object aT in mI.GetCustomAttributes(false))
            {
                if (string.Compare(aT.ToString(), "Xeora.Web.Basics.Attribute.HttpMethodAttribute", true) == 0)
                {
                    Type workingType = aT.GetType();

                    object httpMethod =
                        workingType.InvokeMember("Method", BindingFlags.GetProperty, null, aT, null);
                    object bindProcedureName =
                        workingType.InvokeMember("BindProcedureName", BindingFlags.GetProperty, null, aT, null);

                    return (string.Compare(httpMethod.ToString(), this._HttpMethodType) == 0) && (string.Compare(bindProcedureName.ToString(), this._SearchName) == 0);
                }
            }

            return attributeCheck;
        }
    }
}
