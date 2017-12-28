using System;
using System.Reflection;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]
namespace Xeora.Web.Basics
{
    [CLSCompliant(true)]
    public class Execution
    {
        public static object CrossCall(string executableName, string className, string procedureName, params object[] parameterValues) =>
            Execution.CrossCall(Helpers.CurrentDomainInstance.IDAccessTree, executableName, className, procedureName, parameterValues);

        public static object CrossCall(string[] domainIDAccessTree, string executableName, string className, string procedureName, params object[] parameterValues)
        {
            Assembly webManagerAsm = Assembly.Load("Xeora.Web");
            Type objAssembly = 
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
                object resultObject = 
                    objAssembly.InvokeMember("InvokeBind", BindingFlags.InvokeMethod, null, null, new object[] { bindInfo });

                return ((BindInvokeResult<object>)resultObject).Result;
            }
            catch (Exception ex)
            {
                return new Exception("CrossCall Execution Error!", ex);
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
            private string _RequestHttpMethod;

            private BindInfo(string executableName, string[] classNames, string procedureName, string[] procedureParams)
            {
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

            public string RequestHttpMethod
            {
                get
                {
                    if (string.IsNullOrEmpty(this._RequestHttpMethod))
                    {
                        Context.IHttpContext context = Helpers.Context;

                        if (context != null)
                            this._RequestHttpMethod = context.Request.Header.Method.ToString();
                        else
                            this._RequestHttpMethod = "GET";
                    }

                    return this._RequestHttpMethod;
                }
                set { this._RequestHttpMethod = value; }
            }

            public string ExecutableName { get; private set; }
            public string[] ClassNames { get; private set; }
            public string ProcedureName { get; private set; }
            public ProcedureParameter[] ProcedureParams { get; private set; }
            public object[] ProcedureParamValues { get; private set; }

            public bool IsReady { get; private set; }
            public bool InstanceExecution { get; set; }

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

                            return new BindInfo(executableName, classNames, procedureName, procedureParams);
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

            public void Clone(out BindInfo bindInfo)
            {
                bindInfo = new BindInfo(this.ExecutableName, this.ClassNames, this.ProcedureName, null);

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

                public string Key { get; private set; }
                public object Value { get; set; }
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
