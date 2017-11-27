using System.Collections;
using System.Globalization;
using System.Reflection;

namespace Xeora.Web.Manager
{
    internal class MethodInfoNameComparer : IComparer
    {
        private CultureInfo _CompareCultureInfo;

        public MethodInfoNameComparer(CultureInfo cultureInfo)
        {
            this._CompareCultureInfo = cultureInfo;

            if (this._CompareCultureInfo == null)
                this._CompareCultureInfo = new CultureInfo("en-US");
        }

        public int Compare(object x, object y)
        {
            MethodInfo xObj = (MethodInfo)x;
            MethodInfo yObj = (MethodInfo)y;

            return string.Compare(xObj.Name, yObj.Name, this._CompareCultureInfo, CompareOptions.IgnoreCase);
        }
    }
}
