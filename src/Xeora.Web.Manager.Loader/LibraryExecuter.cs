using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

namespace Xeora.Web.Manager
{
    internal class LibraryExecuter : AssemblyLoadContext, IDisposable
    {
        private string _ExecutableName;
        private string[] _AssemblySearchPaths;

        private string _ExecutablePath;
        private Assembly _AssemblyDll;
        private Type[] _XeoraControls = null;

        private ConcurrentDictionary<Type, object> _ExecutableInstances;
        private ConcurrentDictionary<string, MethodInfo[]> _AssemblyMethods;

        public LibraryExecuter(string executablesPath, string executableName, string[] assemblySearchPaths)
        {
            if (string.IsNullOrEmpty(executablesPath))
                throw new ArgumentNullException(nameof(executablesPath));
            if (string.IsNullOrEmpty(executableName))
                throw new ArgumentNullException(nameof(executableName));

            this.Resolving += this.ResolveAssemblyAgain;

            this._ExecutableName = executableName;
            this._AssemblySearchPaths = assemblySearchPaths;
            if (this._AssemblySearchPaths == null)
                this._AssemblySearchPaths = new string[] {};
            this._ExecutablePath =
                Path.Combine(executablesPath, string.Format("{0}.dll", this._ExecutableName));

            this._ExecutableInstances = new ConcurrentDictionary<Type, object>();
            this._AssemblyMethods = new ConcurrentDictionary<string, MethodInfo[]>();

            this.PrepareXeoraControlTypes();
            this.Load();
        }

        private string ResolveAssemblyLocation(AssemblyName assemblyName)
        {
            string assemblyShortName =
                assemblyName.Name;

            int comaIndex = assemblyShortName.IndexOf(',');
            if (comaIndex > -1)
                assemblyShortName = assemblyShortName.Substring(0, comaIndex);
            
            foreach (string path in this._AssemblySearchPaths)
            {
                string assemblyLocation =
                    Path.Combine(path, string.Format("{0}.dll", assemblyShortName));

                if (File.Exists(assemblyLocation))
                    return assemblyLocation;
            }

            return string.Empty;
        }

        private Assembly SearchInAppDomain(AssemblyName assemblyName)
        {
            Assembly[] currentDomainAssemblies =
                AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in currentDomainAssemblies)
            {
                if (string.Compare(assembly.FullName, assemblyName.FullName) == 0)
                    return assembly;
            }

            return null;
        }

        private Assembly ResolveAssemblyAgain(AssemblyLoadContext sender, AssemblyName assemblyName)
        {
            Assembly assemblyResult =
                this.SearchInAppDomain(assemblyName);

            if (assemblyResult != null)
                return assemblyResult;
            
            string assemblyLocation = 
                this.ResolveAssemblyLocation(assemblyName);
            
            if (!string.IsNullOrEmpty(assemblyLocation))
                return sender.LoadFromAssemblyPath(assemblyLocation);

            return null;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            Assembly assemblyResult =
                this.SearchInAppDomain(assemblyName);

            if (assemblyResult != null)
                return assemblyResult;

            string assemblyLocation = 
                this.ResolveAssemblyLocation(assemblyName);

            if (!string.IsNullOrEmpty(assemblyLocation))
                return Assembly.LoadFrom(assemblyLocation);

            return null;
        }

        private void Load()
        {
            try
            {
                this._AssemblyDll = this.LoadFromAssemblyPath(this._ExecutablePath);
            }
            catch (FileNotFoundException)
            {
                this.MissingFileException = true;

                return;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void PrepareXeoraControlTypes()
        {
            try
            {
                Assembly[] loadedAssemblies =
                    AppDomain.CurrentDomain.GetAssemblies();

                if (loadedAssemblies == null)
                    return;

                foreach (Assembly assembly in loadedAssemblies)
                {
                    if (string.Compare(assembly.GetName().Name, "Xeora.Web.Basics") == 0)
                    {
                        List<Type> XeoraControlTypes = new List<Type>();

                        foreach (Type Type in assembly.GetTypes())
                        {
                            if (string.Compare(Type.Namespace, "Xeora.Web.Basics.ControlResult") == 0)
                                XeoraControlTypes.Add(Type);
                        }

                        this._XeoraControls = XeoraControlTypes.ToArray();

                        break;
                    }
                }
            }
            catch (System.Exception)
            {
                // Just Handle Exceptions
            }
        }

        public bool MissingFileException { get; private set; }

        private bool ExamInterface(string interfaceFullName)
        {
            Type[] assemblyInterfaceList = null;
            Type[] assemblyExTypes =
                this._AssemblyDll.GetExportedTypes();

            if (assemblyExTypes == null)
                return false;

            foreach (Type type in assemblyExTypes)
            {
                assemblyInterfaceList = type.GetInterfaces();

                if (assemblyInterfaceList == null)
                    continue;

                foreach (Type @interface in assemblyInterfaceList)
                {

                    if (string.Compare(@interface.FullName, interfaceFullName) == 0)
                        return true;
                }
            }

            return false;
        }

        private System.Exception GetExecutionError(string[] classNames, string functionName, object[] functionParams, System.Exception innerException)
        {
            string compileErrorObject = null;

            if (classNames != null)
            {
                if (functionParams.Length == 0)
                    compileErrorObject = string.Format("{0}?{1}.{2}", this._ExecutableName, string.Join(".", classNames), functionName);
                else
                    compileErrorObject = string.Format("{0}?{1}.{2},[Length:{3}]", this._ExecutableName, string.Join(".", classNames), functionName, functionParams.Length);
            }
            else
            {
                if (functionParams.Length == 0)
                    compileErrorObject = string.Format("{0}?{1}", this._ExecutableName, functionName);
                else
                    compileErrorObject = string.Format("{0}?{1},[Length:{2}]", this._ExecutableName, functionName, functionParams.Length);
            }

            return new System.Exception(
                string.Format("Executable Execution Error! RequestInfo: {0}", compileErrorObject), innerException);
        }

        private void InvokePreExecution(object executeObject, string executionID, MethodInfo assemblyMethod)
        {
            try
            {
                executeObject.GetType().GetMethod("PreExecute").Invoke(executeObject, new object[] { executionID, assemblyMethod });
            }
            catch (System.Exception)
            {
                // Errors are irrelevant, do not check!
            }
        }

        private void InvokePostExecution(object executeObject, string executionID, object result)
        {
            try
            {
                executeObject.GetType().GetMethod("PostExecute").Invoke(executeObject, new object[] { executionID, result });
            }
            catch (System.Exception)
            {
                // Errors are irrelevant, do not check!
            }
        }

        private object LoadDomainExecutable()
        {
            Type examInterface =
                this._AssemblyDll.GetType(string.Format("Xeora.Domain.{0}", this._ExecutableName), false, true);

            if (examInterface == null)
                return new System.Exception("Assembly does not belong to any XeoraCube Domain or Addon!");

            Type interfaceType =
                examInterface.GetInterface("IDomainExecutable");

            if (interfaceType == null ||
                !interfaceType.IsInterface ||
                string.Compare(interfaceType.FullName, "Xeora.Web.Basics.IDomainExecutable") != 0)
                return new System.Exception("Calling Assembly is not a XeoraCube Executable!");

            object executeObject = null;
            if (!this._ExecutableInstances.TryGetValue(examInterface, out executeObject))
            {
                try
                {
                    executeObject = Activator.CreateInstance(examInterface);

                    if (executeObject != null)
                        this._ExecutableInstances.AddOrUpdate(examInterface, executeObject, (cType, cObject) => executeObject);

                    executeObject.GetType().GetMethod("Initialize").Invoke(executeObject, null);
                }
                catch (System.Exception ex)
                {
                    return new System.Exception("Unable to create an instance of XeoraCube Executable!", ex);
                }
            }

            return executeObject;
        }

        private bool CheckFunctionResultTypeIsXeoraControl(Type methodReturnType)
        {
            if (this._XeoraControls != null && methodReturnType != null)
            {
                foreach (Type xeoraType in this._XeoraControls)
                {
                    if (object.ReferenceEquals(xeoraType, methodReturnType))
                        return true;
                }
            }

            return false;
        }

        private bool FixFunctionParameter(Type parameterType, ref object functionParam)
        {
            if (functionParam == null)
            {
                if (parameterType.Equals(typeof(byte)) ||
                    parameterType.Equals(typeof(sbyte)) ||
                    parameterType.Equals(typeof(short)) ||
                    parameterType.Equals(typeof(ushort)) ||
                    parameterType.Equals(typeof(int)) ||
                    parameterType.Equals(typeof(uint)) ||
                    parameterType.Equals(typeof(long)) ||
                    parameterType.Equals(typeof(ulong)) ||
                    parameterType.Equals(typeof(double)) ||
                    parameterType.Equals(typeof(float)))
                    functionParam = 0;

                return true;
            }

            if (object.ReferenceEquals(parameterType, functionParam.GetType()))
                return true;
            if (parameterType.IsAssignableFrom(functionParam.GetType()))
                return true;
            if (string.Compare(parameterType.FullName, typeof(object).FullName, true) == 0)
                return true;

            try
            {
                functionParam = Convert.ChangeType(functionParam, parameterType);
                return true;
            }
            catch (System.Exception)
            {
                if (functionParam is string && 
                    string.IsNullOrEmpty((string)functionParam))
                {
                    functionParam = null;
                    return this.FixFunctionParameter(parameterType, ref functionParam);
                }
            }

            return false;
        }

        private MethodInfo GetAssemblyMethod(ref Type assemblyObject, string httpMethodType, string functionName, ref object[] functionParams, string executerType)
        {
            MethodInfoFinder mIF = 
                new MethodInfoFinder(httpMethodType, functionName);
            string searchKey =
                string.Format("{0}.{1}", assemblyObject.FullName, mIF.Identifier);

            MethodInfo[] possibleMethodInfos = null;
            if (!this._AssemblyMethods.TryGetValue(searchKey, out possibleMethodInfos))
            {
                possibleMethodInfos =
                    Array.FindAll<MethodInfo>(assemblyObject.GetMethods(), new Predicate<MethodInfo>(mIF.Find));

                this._AssemblyMethods.AddOrUpdate(searchKey, possibleMethodInfos, (cKey, cValue) => possibleMethodInfos);
            }

            MethodInfo workingMethodInfo;
            object[] functionParamsReBuild;

            for (int mC = 0; mC < possibleMethodInfos.Length; mC++)
            {
                workingMethodInfo = possibleMethodInfos[mC];
                functionParamsReBuild = (object[])functionParams.Clone();

                bool isXeoraControl =
                    this.CheckFunctionResultTypeIsXeoraControl(workingMethodInfo.ReturnType);
                ParameterInfo[] mParams = workingMethodInfo.GetParameters();

                switch (executerType)
                {
                    case "Control":
                        if (!object.ReferenceEquals(workingMethodInfo.ReturnType, typeof(object)) && !isXeoraControl)
                            continue;

                        break;
                    case "Other":
                        if (isXeoraControl)
                        {
                            switch (workingMethodInfo.ReturnType.Name)
                            {
                                case "RedirectOrder":
                                case "Message":
                                    // These are exceptional controls
                                    break;
                                default:
                                    continue;
                            }
                        }

                        break;
                }

                if (mParams.Length == 0 && functionParamsReBuild.Length == 0)
                {
                    functionParams = functionParamsReBuild;
                    return workingMethodInfo;
                }

                if (mParams.Length > 0 && mParams.Length <= functionParamsReBuild.Length)
                {
                    bool matchComplete = false;
                    bool[] isExactMatch = new bool[mParams.Length];

                    for (int pC = 0; pC < mParams.Length; pC++)
                    {
                        if (pC != mParams.Length - 1)
                        {
                            isExactMatch[pC] = this.FixFunctionParameter(mParams[pC].ParameterType, ref functionParamsReBuild[pC]);

                            continue;
                        }

                        bool checkIsParamArrayDefined =
                            Attribute.IsDefined(mParams[pC], typeof(ParamArrayAttribute));

                        if (!checkIsParamArrayDefined)
                        {
                            isExactMatch[pC] = this.FixFunctionParameter(mParams[pC].ParameterType, ref functionParamsReBuild[pC]);

                            if (mParams.Length == functionParamsReBuild.Length && Array.IndexOf(isExactMatch, false) == -1)
                                matchComplete = true;

                            break;
                        }

                        Array paramArrayValues =
                            Array.CreateInstance(mParams[pC].ParameterType.GetElementType(), (functionParamsReBuild.Length - mParams.Length) + 1);

                        for (int pavC = pC; pavC < functionParamsReBuild.Length; pavC++)
                        {
                            this.FixFunctionParameter(mParams[pC].ParameterType.GetElementType(), ref functionParamsReBuild[pavC]);

                            paramArrayValues.SetValue(functionParamsReBuild[pavC], pavC - (mParams.Length - 1));
                        }

                        Array.Resize(ref functionParamsReBuild, mParams.Length);
                        functionParamsReBuild[pC] = paramArrayValues;

                        isExactMatch[pC] = true;
                        matchComplete = true;

                        break;
                    }

                    if (matchComplete && Array.IndexOf(isExactMatch, false) == -1)
                    {
                        functionParams = functionParamsReBuild;
                        return workingMethodInfo;
                    }
                }
            }

            if (assemblyObject.BaseType != null)
            {
                Type baseType = assemblyObject.BaseType;
                MethodInfo assemblyMethod =
                    this.GetAssemblyMethod(ref baseType, httpMethodType, functionName, ref functionParams, executerType);

                if (assemblyMethod != null)
                {
                    assemblyObject = assemblyObject.BaseType;
                    return assemblyMethod;
                }
            }

            return null;
        }

        private System.Exception GetMethodException(string httpMethodType, string[] classNames, string functionName, object[] functionParams)
        {
            System.Text.StringBuilder sB = new System.Text.StringBuilder();

            sB.AppendLine("Assembly does not have following procedure!");
            sB.AppendLine("--------------------------------------------------");
            sB.AppendFormat("ExecutableName: {0}", this._ExecutableName);
            sB.AppendLine();
            sB.AppendFormat("ClassName: {0}", string.Join(".", classNames));
            sB.AppendLine();
            sB.AppendFormat("FunctionName: {0}", functionName);
            sB.AppendLine();
            sB.AppendFormat("FunctionParamsLength: {0}", functionParams.Length);
            foreach (object Param in functionParams)
            {
                sB.AppendFormat(", {0}", Param.GetType().ToString());
            }
            sB.AppendLine();
            sB.AppendFormat("HttpMethod: {0}", httpMethodType);
            sB.AppendLine();

            return new TargetException(sB.ToString());
        }

        private object InvokeMethod(bool instanceExecute, Type assemblyObject, MethodInfo assemblyMethod, object[] functionParams)
        {
            if (!instanceExecute)
                return assemblyMethod.Invoke(assemblyObject, BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, functionParams, System.Threading.Thread.CurrentThread.CurrentCulture);

            object instanceObject = null;
            if (!this._ExecutableInstances.TryGetValue(assemblyObject, out instanceObject))
            {
                try
                {
                    instanceObject = Activator.CreateInstance(assemblyObject);

                    if (instanceObject != null)
                        this._ExecutableInstances.AddOrUpdate(assemblyObject, instanceObject, (cType, cObject) => instanceObject);
                }
                catch (System.Exception ex)
                {
                    return new System.Exception("Unable to create an instance of XeoraCube Executable Class!", ex);
                }
            }

            if (instanceObject == null)
                return new TargetException("Execution can not be processed on null instance!");

            return assemblyMethod.Invoke(instanceObject, BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, functionParams, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public object Invoke(string httpMethodType, string[] classNames, string functionName, object[] functionParams, bool instanceExecute, string executerType)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException(nameof(functionName));
            if (functionParams == null)
                functionParams = new object[] { };

            string executionID = Guid.NewGuid().ToString();
            object result = null;

            object executeObject =
                this.LoadDomainExecutable();

            if (executeObject is System.Exception)
                return executeObject;

            try
            {
                Type assemblyObject = null;
                if (classNames != null)
                    assemblyObject = this._AssemblyDll.GetType(string.Format("Xeora.Domain.{0}", string.Join("+", classNames)), true, true);
                else
                    assemblyObject = executeObject.GetType();

                MethodInfo assemblyMethod =
                    this.GetAssemblyMethod(ref assemblyObject, httpMethodType, functionName, ref functionParams, executerType);

                if (assemblyMethod == null)
                    return this.GetMethodException(httpMethodType, classNames, functionName, functionParams);

                this.InvokePreExecution(executeObject, executionID, assemblyMethod);

                result = this.InvokeMethod(instanceExecute, assemblyObject, assemblyMethod, functionParams);

                return result;
            }
            catch (System.Exception ex)
            {
                return this.GetExecutionError(classNames, functionName, functionParams, ex);
            }
            finally
            {
                this.InvokePostExecution(executeObject, executionID, result);
            }
        }

        public void Dispose()
        {
            Type examInterface =
                this._AssemblyDll.GetType(string.Format("Xeora.Domain.{0}", this._ExecutableName), false, true);

            object executeObject = null;
            if (this._ExecutableInstances.TryRemove(examInterface, out executeObject))
            {
                try
                {
                    executeObject.GetType().GetMethod("Finalize").Invoke(executeObject, null);
                }
                catch (System.Exception)
                {
                    //Throw New System.Exception("XeoraCube Executable execution error while finalizing", ex)
                    // Just handle Exception and do not throw and exception
                }
            }
        }
    }
}
