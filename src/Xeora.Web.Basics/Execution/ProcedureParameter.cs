using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics.Execution
{
    [Serializable()]
    public class ProcedureParameter
    {
        private Dictionary<char, bool> _Operators =
            new Dictionary<char, bool>() {
                { '^', true },
                { '~', true },
                { '-', true },
                { '+', true },
                { '=', true },
                { '#', true },
                { '*', true }
            };

        public ProcedureParameter(string parameter)
        {
            this.Query = string.Empty;
            this.Key = string.Empty;
            this.Value = null;

            if (!string.IsNullOrEmpty(parameter))
            {
                this.Query = parameter;
                this.Key = parameter;

                if (!this._Operators.ContainsKey(this.Key[0]))
                    return;

                if (this.Key[0] != '#')
                    this.Key = this.Key.Substring(1);
                else
                {
                    do
                    {
                        if (this.Key[0] != '#')
                            break;

                        this.Key = this.Key.Substring(1);
                    } while (this.Key.Length > 0);
                }
            }
        }

        /// <summary>
        /// Gets the key of the parameter with operator
        /// </summary>
        /// <value>Parameter query</value>
        public string Query { get; private set; }

        /// <summary>
        /// Gets the key of the parameter without operator
        /// </summary>
        /// <value>Parameter key</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        /// <value>Parameter value</value>
        public object Value { get; set; }
    }
}
