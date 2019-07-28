using System.Collections;
using System.Reflection;

namespace Xeora.Web.Manager
{
    internal class MethodInfoParameterLengthComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            MethodInfo xObj = (MethodInfo)x;
            MethodInfo yObj = (MethodInfo)y;

            ParameterInfo[] xObjParams = xObj.GetParameters();
            ParameterInfo[] yObjParams = yObj.GetParameters();

            if (xObjParams.Length > yObjParams.Length)
                return 1;
            if (xObjParams.Length < yObjParams.Length)
                return -1;

            return 0;
        }
    }
}
