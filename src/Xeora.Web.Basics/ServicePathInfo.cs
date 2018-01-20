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

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Xeora.Web.Basics.ServicePathInfo"/> is mapped
        /// </summary>
        /// <value><c>true</c> if is mapped; otherwise, <c>false</c></value>
        public bool IsMapped { get; private set; }

        /// <summary>
        /// Gets the path tree
        /// </summary>
        /// <value>The path tree of service info</value>
        public LinkedList<string> PathTree { get; private set; }

        /// <summary>
        /// Gets the service identifier
        /// </summary>
        /// <value>The service identifier</value>
        public string ServiceID { get; private set; }

        private string _FullPath = null;
        /// <summary>
        /// Gets the full path including service identifier
        /// </summary>
        /// <value>The full path</value>
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

        /// <summary>
        /// Parse the specified fullPath into <see cref="T:Xeora.Web.Basics.ServicePathInfo"/>
        /// </summary>
        /// <returns>The ServicePathInfo object</returns>
        /// <param name="fullPath">Full path</param>
        /// <param name="isMapped">If set to <c>true</c> is marked as mapped</param>
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
