using System;

namespace Xeora.Web.Basics.Execution
{
    [Serializable]
    public class Bind
    {
        private Bind(string executable, string[] classes, string procedure, string[] parameters)
        {
            this.Executable = executable;
            this.Classes = classes;
            this.Procedure = procedure;
            this.Parameters = new ProcedureParameterCollection(parameters);
            this.InstanceExecution = false;
        }

        /// <summary>
        /// Gets the name of the xeora executable
        /// </summary>
        /// <value>The name of the xeora executable</value>
        public string Executable { get; }

        /// <summary>
        /// Gets the class tree from top to bottom
        /// </summary>
        /// <value>The class tree</value>
        public string[] Classes { get; }

        /// <summary>
        /// Gets the name of the procedure
        /// </summary>
        /// <value>The name of the procedure</value>
        public string Procedure { get; }

        /// <summary>
        /// Gets the procedure parameters
        /// </summary>
        /// <value>The procedure parameters</value>
        public ProcedureParameterCollection Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Xeora.Web.Basics.Execution.Bind"/> is ready
        /// </summary>
        /// <value><c>true</c> if is ready; otherwise, <c>false</c></value>
        public bool Ready => this.Parameters.Healthy;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Xeora.Web.Basics.Execution.Bind"/>
        /// instance execution. If the class requires instance creation, make it <c>true</c>
        /// </summary>
        /// <value><c>true</c> if instance execution; otherwise, <c>false</c></value>
        public bool InstanceExecution { get; set; }

        /// <summary>
        /// Make the Bind from string
        /// </summary>
        /// <returns>The Bind</returns>
        /// <param name="bind">Bind string</param>
        public static Bind Make(string bind)
        {
            if (string.IsNullOrEmpty(bind))
                return null;

            string[] bindLocationParts = bind.Split('?');

            if (bindLocationParts.Length != 2)
                return null;

            string executable = bindLocationParts[0];
            string[] bindExecutionParts = bindLocationParts[1].Split(',');

            string[] classes = null;
            string procedure;

            string[] classProcSearch = bindExecutionParts[0].Split('.');

            if (classProcSearch.Length == 1)
                procedure = classProcSearch[0];
            else
            {
                classes = new string[classProcSearch.Length - 1];
                Array.Copy(classProcSearch, 0, classes, 0, classes.Length);

                procedure = classProcSearch[^1];
            }

            string[] parameters = null;
            if (bindExecutionParts.Length > 1)
                parameters = string.Join(",", bindExecutionParts, 1, bindExecutionParts.Length - 1).Split('|');

            return new Bind(executable, classes, procedure, parameters);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.Execution.Bind"/>
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.Execution.Bind"/></returns>
        public override string ToString()
        {
            return
                string.Format("{0}?{1}{2}{3}{4}",
                    this.Executable,
                    string.Join(".", this.Classes ?? Array.Empty<string>()),
                    this.Classes == null ? string.Empty : ".",
                    this.Procedure,
                    this.Parameters
                );
        }

        /// <summary>
        /// Clone into the specified bind
        /// </summary>
        /// <param name="bind">Bind object that keeps cloned data</param>
        public void Clone(out Bind bind) =>
            bind = new Bind(this.Executable, this.Classes, this.Procedure, this.Parameters.Queries);
    }
}
