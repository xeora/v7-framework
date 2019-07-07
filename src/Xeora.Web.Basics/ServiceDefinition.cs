using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public class ServiceDefinition
    {
        public ServiceDefinition() : this(string.Empty, false)
        { }

        private ServiceDefinition(string serviceId, bool mapped)
        {
            this.Mapped = mapped;

            this.PathTree = new LinkedList<string>();
            this.ServiceId = serviceId;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Xeora.Web.Basics.ServiceDefinition"/> is mapped
        /// </summary>
        /// <value><c>true</c> if is mapped; otherwise, <c>false</c></value>
        public bool Mapped { get; private set; }

        /// <summary>
        /// Gets the path tree
        /// </summary>
        /// <value>The path tree of service definition</value>
        public LinkedList<string> PathTree { get; private set; }

        /// <summary>
        /// Gets the service identifier
        /// </summary>
        /// <value>The service identifier</value>
        public string ServiceId { get; private set; }

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

                    this._FullPath = string.Concat(this._FullPath, this.ServiceId);
                }

                return this._FullPath;
            }
        }

        /// <summary>
        /// Parse the specified fullPath into <see cref="T:Xeora.Web.Basics.ServiceDefinition"/>
        /// </summary>
        /// <returns>The ServiceDefinition object</returns>
        /// <param name="fullPath">Full path</param>
        /// <param name="mapped">If set to <c>true</c> is marked as mapped</param>
        public static ServiceDefinition Parse(string fullPath, bool mapped)
        {
            string[] requestPaths = fullPath.Split('/');

            ServiceDefinition rServiceDefinition = new ServiceDefinition(requestPaths[requestPaths.Length - 1], mapped);
            for (int pC = 0; pC < requestPaths.Length - 1; pC++)
                rServiceDefinition.PathTree.AddLast(requestPaths[pC]);

            return rServiceDefinition;
        }
    }
}
