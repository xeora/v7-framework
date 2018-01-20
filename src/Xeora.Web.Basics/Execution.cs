using System;
using System.Reflection;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]
namespace Xeora.Web.Basics
{
    [CLSCompliant(true)]
    public class Execution
    {
        /// <summary>
        /// Calls the side Xeora executable
        /// </summary>
        /// <returns>Call result</returns>
        /// <param name="executableName">Xeora executable name</param>
        /// <param name="className">Class name</param>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameterValues">Parameters values</param>
        /// <typeparam name="T">Expected result Type</typeparam>
        public static BindInvokeResult<T> CrossCall<T>(string executableName, string className, string procedureName, params object[] parameterValues) =>
            Execution.CrossCall<T>(Helpers.CurrentDomainInstance.IDAccessTree, executableName, className, procedureName, parameterValues);

        /// <summary>
        /// Calls the side Xeora executable , side Domain or sub domain executable
        /// </summary>
        /// <returns>Call result</returns>
        /// <param name="domainIDAccessTree">DomainID Access tree</param>
        /// <param name="executableName">Xeora executable name</param>
        /// <param name="className">Class name</param>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameterValues">Parameters values</param>
        /// <typeparam name="T">Expected result Type</typeparam>
        public static BindInvokeResult<T> CrossCall<T>(string[] domainIDAccessTree, string executableName, string className, string procedureName, params object[] parameterValues)
        {
            Assembly webManagerAsm = Assembly.Load("Xeora.Web");
            Type assemblyCoreType =
                webManagerAsm.GetType("Xeora.Web.Manager.AssemblyCore", false, true);

            BindInfo bindInfo = null;
            Dictionary<string, object> parametersValueMap =
                new Dictionary<string, object>();

            if (parameterValues == null ||
                parameterValues.Length == 0)
                bindInfo = BindInfo.Make(string.Format("{0}?{1}.{2}", executableName, className, procedureName));
            else
            {
                string[] parametersStructure = new string[parameterValues.Length];

                for (int pC = 0; pC < parameterValues.Length; pC++)
                {
                    string paramName = string.Format("PARAM{0}", pC);

                    parametersValueMap[paramName] = parameterValues[pC];
                    parametersStructure[pC] = paramName;
                }
                bindInfo =
                    BindInfo.Make(
                        string.Format(
                            "{0}?{1}.{2},{3}",
                            executableName,
                            className,
                            procedureName,
                            string.Join("|", parametersStructure)
                        )
                    );
            }

            bindInfo.PrepareProcedureParameters(
                new BindInfo.ProcedureParser(
                    (ref BindInfo.ProcedureParameter param) => { param.Value = parametersValueMap[param.Key]; }
                )
            );

            try
            {
                MethodInfo invokeBindMethod =
                    assemblyCoreType.GetMethod("InvokeBind", new Type[] { typeof(BindInfo) });
                invokeBindMethod = invokeBindMethod.MakeGenericMethod(typeof(T));

                return
                    (Execution.BindInvokeResult<T>)invokeBindMethod.Invoke(null, new object[] { bindInfo });
            }
            catch (Exception ex)
            {
                Execution.BindInvokeResult<T> rBindInvokeResult =
                    new Execution.BindInvokeResult<T>(bindInfo);
                rBindInvokeResult.Exception = new Exception("CrossCall Execution Error!", ex);

                return rBindInvokeResult;
            }
        }

        public static string GetPrimitiveValue(object methodResult)
        {
            if (methodResult != null &&
                (methodResult.GetType().IsPrimitive || methodResult is string))
                return (string)methodResult;

            return null;
        }

        [Serializable()]
        public class BindInfo
        {
            private BindInfo(Context.HttpMethod httpMethod, string executableName, string[] classNames, string procedureName, string[] procedureParams)
            {
                this.HttpMethod = httpMethod;
                this.ExecutableName = executableName;
                this.ClassNames = classNames;
                this.ProcedureName = procedureName;

                this.ProcedureParams = null;
                if (procedureParams != null)
                {
                    this.ProcedureParams = new ProcedureParameter[procedureParams.Length];

                    for (int pC = 0; pC < procedureParams.Length; pC++)
                        this.ProcedureParams[pC] = new ProcedureParameter(procedureParams[pC]);
                }

                this.IsReady = false;
                this.InstanceExecution = false;
            }

            /// <summary>
            /// Gets or sets the request http method
            /// </summary>
            /// <value>The request http method</value>
            public Context.HttpMethod HttpMethod { get; private set; }

            /// <summary>
            /// Gets the name of the xeora executable
            /// </summary>
            /// <value>The name of the xeora executable</value>
            public string ExecutableName { get; private set; }

            /// <summary>
            /// Gets the class tree from top to bottom
            /// </summary>
            /// <value>The class tree</value>
            public string[] ClassNames { get; private set; }

            /// <summary>
            /// Gets the name of the procedure
            /// </summary>
            /// <value>The name of the procedure</value>
            public string ProcedureName { get; private set; }

            /// <summary>
            /// Gets the procedure parameter names
            /// </summary>
            /// <value>The procedure parameter names</value>
            public ProcedureParameter[] ProcedureParams { get; private set; }

            /// <summary>
            /// Gets the procedure parameter values
            /// </summary>
            /// <value>The procedure parameter values</value>
            public object[] ProcedureParamValues { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="T:Xeora.Web.Basics.Execution.BindInfo"/> is ready
            /// </summary>
            /// <value><c>true</c> if is ready; otherwise, <c>false</c></value>
            public bool IsReady { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="T:Xeora.Web.Basics.Execution.BindInfo"/>
            /// instance execution. If the class requires instance creation, make it <c>true</c>
            /// </summary>
            /// <value><c>true</c> if instance execution; otherwise, <c>false</c></value>
            public bool InstanceExecution { get; set; }

            /// <summary>
            /// Overrides and reorganizes the procedure parameter names
            /// </summary>
            /// <param name="procedureParams">Procedure parameter names</param>
            public void OverrideProcedureParameters(string[] procedureParams)
            {
                this.ProcedureParams = null;
                if (procedureParams != null)
                {
                    this.ProcedureParams = new ProcedureParameter[procedureParams.Length];

                    for (int pC = 0; pC < procedureParams.Length; pC++)
                    {
                        this.ProcedureParams[pC] = new ProcedureParameter(procedureParams[pC]);
                    }
                }

                this.ProcedureParamValues = null;
                this.IsReady = false;
            }

            public delegate void ProcedureParser(ref ProcedureParameter procedureParameter);
            /// <summary>
            /// Prepares the procedure parameter values
            /// </summary>
            /// <param name="procedureParser">Procedure parser</param>
            public void PrepareProcedureParameters(ProcedureParser procedureParser)
            {
                if (procedureParser != null)
                {
                    if (this.ProcedureParams != null)
                    {
                        this.ProcedureParamValues = new object[this.ProcedureParams.Length];

                        for (int pC = 0; pC < this.ProcedureParams.Length; pC++)
                        {
                            procedureParser.Invoke(ref this.ProcedureParams[pC]);

                            this.ProcedureParamValues[pC] = this.ProcedureParams[pC].Value;
                        }
                    }

                    this.IsReady = true;
                }
            }

            /// <summary>
            /// Make the BindInfo from string
            /// </summary>
            /// <returns>The BindInfo</returns>
            /// <param name="bind">Bind string</param>
            public static BindInfo Make(string bind)
            {
                if (!string.IsNullOrEmpty(bind))
                {
                    try
                    {
                        string[] splittedBindInfo1 = bind.Split('?');

                        if (splittedBindInfo1.Length == 2)
                        {
                            string executableName = splittedBindInfo1[0];
                            string[] splittedBindInfo2 = splittedBindInfo1[1].Split(',');

                            string[] classNames = null;
                            string procedureName = null;

                            string[] classProcSearch = splittedBindInfo2[0].Split('.');

                            if (classProcSearch.Length == 1)
                            {
                                classNames = null;
                                procedureName = classProcSearch[0];
                            }
                            else
                            {
                                classNames = new string[classProcSearch.Length - 1];
                                Array.Copy(classProcSearch, 0, classNames, 0, classNames.Length);

                                procedureName = classProcSearch[classProcSearch.Length - 1];
                            }

                            string[] procedureParams = null;
                            if (splittedBindInfo2.Length > 1)
                                procedureParams = string.Join(",", splittedBindInfo2, 1, splittedBindInfo2.Length - 1).Split('|');

                            return new BindInfo(Helpers.Context.Request.Header.Method, executableName, classNames, procedureName, procedureParams);
                        }
                    }
                    catch (Exception)
                    {
                        // Just Handle Exceptions
                    }
                }

                return null;
            }

            private string ProvideProcedureParameters()
            {
                System.Text.StringBuilder rString = new System.Text.StringBuilder();

                for (int pC = 0; pC < this.ProcedureParams.Length; pC++)
                {
                    rString.Append(this.ProcedureParams[pC].Query);

                    if (pC < (this.ProcedureParams.Length - 1))
                        rString.Append("|");
                }

                return rString.ToString();
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.Execution.BindInfo"/>
            /// </summary>
            /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Xeora.Web.Basics.Execution.BindInfo"/></returns>
            public override string ToString()
            {
                string rString =
                    string.Format("{0}?{1}{2}{3}",
                        this.ExecutableName,
                        string.Join(".", this.ClassNames),
                        (this.ClassNames == null ? string.Empty : "."),
                        this.ProcedureName
                    );

                if (this.ProcedureParams != null)
                    rString = string.Format("{0},{1}", rString, this.ProvideProcedureParameters());

                return rString;
            }

            /// <summary>
            /// Clone to the specified bindInfo
            /// </summary>
            /// <param name="bindInfo">BindInfo object that keeps cloned data</param>
            public void Clone(out BindInfo bindInfo)
            {
                bindInfo = new BindInfo(this.HttpMethod, this.ExecutableName, this.ClassNames, this.ProcedureName, null);

                if (this.ProcedureParams != null)
                {
                    bindInfo.ProcedureParams = new ProcedureParameter[this.ProcedureParams.Length];
                    Array.Copy(this.ProcedureParams, bindInfo.ProcedureParams, this.ProcedureParams.Length);
                }
            }

            [Serializable()]
            public class ProcedureParameter
            {
                public ProcedureParameter(string procedureParameter)
                {
                    this.Key = string.Empty;
                    this.Value = null;
                    this.Query = string.Empty;

                    if (!string.IsNullOrEmpty(procedureParameter))
                    {
                        this.Query = procedureParameter;

                        char[] operatorChars = new char[] { '^', '~', '-', '+', '=', '#', '*' };

                        if (Array.IndexOf(operatorChars, procedureParameter[0]) > -1)
                        {
                            if (procedureParameter[0] != '#')
                                this.Key = procedureParameter.Substring(1);
                            else
                            {
                                for (int cC = 0; cC < procedureParameter.Length; cC++)
                                {
                                    if (procedureParameter[cC] != '#')
                                    {
                                        this.Key = procedureParameter.Substring(cC);

                                        break;
                                    }
                                }
                            }

                            return;
                        }

                        this.Key = this.Query;
                    }
                }

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

                /// <summary>
                /// Gets the key of the parameter with operator
                /// </summary>
                /// <value>Parameter query</value>
                public string Query { get; private set; }
            }
        }

        [CLSCompliant(true), Serializable()]
        public class BindInvokeResult<T>
        {
            public BindInvokeResult(BindInfo bind)
            {
                this.Bind = bind;
                this.Result = default(T);
                this.Exception = null;
                this.ApplicationPath = string.Empty;
            }

            public BindInfo Bind { get; private set; }
            public T Result { get; set; }
            public Exception Exception { get; set; }
            public string ApplicationPath { get; set; }
        }
    }
}
