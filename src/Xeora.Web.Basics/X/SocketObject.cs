using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics.X
{
    [Serializable]
    public class SocketObject
    {
        public SocketObject(ref Context.IHttpContext context, KeyValuePair<string, object>[] parameters)
        {
            this.Request = context.Request;
            this.Response = context.Response;
            this.Parameters = new SocketParameterCollection(parameters);
        }

        /// <summary>
        /// Gets the context request
        /// </summary>
        /// <value>The context request</value>
        public Context.IHttpRequest Request { get; }

        /// <summary>
        /// Gets the context response
        /// </summary>
        /// <value>The context response</value>
        public Context.IHttpResponse Response { get; }

        /// <summary>
        /// Gets the parameters defined in Configurations.xml
        /// </summary>
        /// <value>The parameters</value>
        public SocketParameterCollection Parameters { get; }
    }
}
