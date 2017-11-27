using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public class ServicePathInfo
    {
        public ServicePathInfo() : this(string.Empty, false)
        { }

        private ServicePathInfo(string serviceID, bool isMapped)
        {
            this.IsMapped = isMapped;

            this.PathTree = new LinkedList<string>();
            this.ServiceID = serviceID;
        }

        public bool IsMapped { get; private set; }
        public LinkedList<string> PathTree { get; private set; }
        public string ServiceID { get; private set; }

        private string _FullPath = null;
        public string FullPath
        {
            get
            {
                if (this._FullPath == null)
                {
                    string[] pathTreeArr = new string[this.PathTree.Count];
                    this.PathTree.CopyTo(pathTreeArr, 0);

                    this._FullPath = string.Join("/", pathTreeArr);

                    if (!string.IsNullOrEmpty(this._FullPath))
                        this._FullPath = string.Concat(this._FullPath, "/");

                    this._FullPath = string.Concat(this._FullPath, this.ServiceID);
                }

                return this._FullPath;
            }
        }

        public static ServicePathInfo Parse(string fullPath, bool isMapped)
        {
            string[] requestPaths = fullPath.Split('/');

            ServicePathInfo rServicePathInfo = new ServicePathInfo(requestPaths[requestPaths.Length - 1], isMapped);
            for (int pC = 0; pC < requestPaths.Length - 1; pC++)
                rServicePathInfo.PathTree.AddLast(requestPaths[pC]);

            return rServicePathInfo;
        }
    }
}
