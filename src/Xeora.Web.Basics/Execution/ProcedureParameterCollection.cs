using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Xeora.Web.Basics.Execution
{
    [Serializable]
    public class ProcedureParameterCollection : IEnumerable
    {
        private readonly List<ProcedureParameter> _Parameters;

        public ProcedureParameterCollection(string[] parameters)
        {
            this._Parameters = new List<ProcedureParameter>();

            this.Override(parameters);
        }

        public ProcedureParameter this[int index] =>
            this._Parameters[index];

        public string[] Queries
        {
            get
            {
                string[] queries = new string[this._Parameters.Count];

                for (int pC = 0; pC < this._Parameters.Count; pC++)
                    queries[pC] = this._Parameters[pC].Query;

                return queries;
            }
        }

        public object[] Values
        {
            get
            {
                if (!this.Healthy)
                    throw new Exception("Collection is not healthy!");

                object[] values = new object[this._Parameters.Count];

                for (int pC = 0; pC < this._Parameters.Count; pC++)
                    values[pC] = this._Parameters[pC].Value;

                return values;
            }
        }

        public bool Healthy { get; private set; }

        public int Count => this._Parameters.Count;

        /// <summary>
        /// Overrides and reorganizes the procedure parameters
        /// </summary>
        /// <param name="parameters">Procedure parameter names</param>
        public void Override(string[] parameters)
        {
            this._Parameters.Clear();
            this.Healthy = false;

            if (parameters == null)
                return;
            
            foreach (var parameter in parameters)
                this._Parameters.Add(new ProcedureParameter(parameter));
        }

        /// <summary>
        /// Prepares the procedure parameter values
        /// </summary>
        /// <param name="parser">Procedure parser</param>
        public void Prepare(Func<ProcedureParameter, object> parser)
        {
            if (parser == null)
                return;
            
            foreach (ProcedureParameter parameter in this._Parameters)
                parameter.Value = parser.Invoke(parameter);

            this.Healthy = true;
        }

        public override string ToString()
        {
            StringBuilder parameterBuilder =
                new StringBuilder();

            for (int pC = 0; pC < this._Parameters.Count; pC++)
            {
                if (parameterBuilder.Length > 0)
                    parameterBuilder.Append("|");

                if (pC == 0)
                    parameterBuilder.Append(",");

                parameterBuilder.Append(this._Parameters[pC].Query);
            }

            return parameterBuilder.ToString();
        }

        public IEnumerator GetEnumerator() =>
            this._Parameters.GetEnumerator();
    }
}
