using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Xeora.Web.Manager
{
    internal class LibraryManager : AssemblyLoadContext, IDisposable
    {
        private readonly string _ExecutableName;
        private readonly string[] _AssemblySearchPaths;

        private readonly string _ExecutablePath;
        private Assembly _AssemblyDll;
        private Dictionary<Type, bool> _XeoraControlTypes;

        private readonly object _InstanceCreationLock;
        private readonly Dictionary<Type, object> _ExecutableInstances;
        
        private readonly object _AssemblyMethodLock;
        private readonly Dictionary<string, MethodInfo[]> _AssemblyMethods;

        public LibraryManager(string executablesPath, string executableName, string[] assemblySearchPaths)
        {
            if (string.IsNullOrEmpty(executablesPath))
                throw new ArgumentNullException(nameof(executablesPath));
            if (string.IsNullOrEmpty(executableName))
                throw new ArgumentNullException(nameof(executableName));

            this.Resolving += this.ResolveAssemblyAgain;

            this._ExecutableName = executableName;
            this._AssemblySearchPaths = assemblySearchPaths ?? new string[] { };
            this._ExecutablePath =
                Path.Combine(executablesPath, $"{this._ExecutableName}.dll");

            this._InstanceCreationLock = new object();
            this._ExecutableInstances = 
                new Dictionary<Type, object>();
            
            this._AssemblyMethodLock = new object();
            this._AssemblyMethods = 
                new Dictionary<string, MethodInfo[]>();

            this.PrepareXeoraControlTypes();
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
                    Path.Combine(path, $"{assemblyShortName}.dll");

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
                if (string.CompareOrdinal(assembly.FullName, assemblyName.FullName) == 0)
                    return assembly;

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

            return !string.IsNullOrEmpty(assemblyLocation) ? sender.LoadFromAssemblyPath(assemblyLocation) : null;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            Assembly assemblyResult =
                this.SearchInAppDomain(assemblyName);

            if (assemblyResult != null)
                return assemblyResult;

            string assemblyLocation =
                this.ResolveAssemblyLocation(assemblyName);

            return !string.IsNullOrEmpty(assemblyLocation) ? Assembly.LoadFrom(assemblyLocation) : null;
        }

        public void Load()
        {
            try
            {
                this._AssemblyDll = this.LoadFromAssemblyPath(this._ExecutablePath);
            }
            catch (FileNotFoundException)
            {
                this.MissingFileException = true;
            }
        }

        private void PrepareXeoraControlTypes()
        {
            this._XeoraControlTypes = new Dictionary<Type, bool>();

            try
            {
                Assembly[] loadedAssemblies =
                    AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in loadedAssemblies)
                {
                    if (string.CompareOrdinal(assembly.GetName().Name, "Xeora.Web.Basics") != 0) continue;
                    
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (string.CompareOrdinal(type.Namespace, "Xeora.Web.Basics.ControlResult") == 0)
                            this._XeoraControlTypes[type] = true;
                    }

                    break;
                }
            }
            catch (Exception)
            {
                // Just Handle Exceptions
            }
        }

        public bool MissingFileException { get; private set; }

        private bool ExamInterface(string interfaceFullName)
        {
            Type[] assemblyExTypes =
                this._AssemblyDll.GetExportedTypes();

            foreach (Type type in assemblyExTypes)
            {
                Type[] assemblyInterfaceList = 
                    type.GetInterfaces();

                foreach (Type @interface in assemblyInterfaceList)
                {
                    if (string.CompareOrdinal(@interface.FullName, interfaceFullName) == 0)
                        return true;
                }
            }

            return false;
        }

        private Exception GetExecutionError(string[] classNames, string functionName, object[] functionParams, Exception innerException)
        {
            string compileErrorObject;

            if (classNames != null)
            {
                compileErrorObject = functionParams.Length == 0 
                    ? $"{this._ExecutableName}?{string.Join(".", classNames)}.{functionName}"
                    : $"{this._ExecutableName}?{string.Join(".", classNames)}.{functionName},[Length:{functionParams.Length}]";
            }
            else
            {
                compileErrorObject = functionParams.Length == 0 
                    ? $"{this._ExecutableName}?{functionName}"
                    : $"{this._ExecutableName}?{functionName},[Length:{functionParams.Length}]";
            }

            return new Exception(
                $"Executable Execution Error! RequestInfo: {compileErrorObject}", innerException);
        }

        private void InvokePreExecution(object executeObject, string executionId, MethodInfo assemblyMethod)
        {
            try
            {
                executeObject.GetType().GetMethod("PreExecute")
                    ?.Invoke(executeObject, new object[] {executionId, assemblyMethod});
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                
                Basics.Console.Push(
                    "Domain PreExecution ERROR", ex.Message, ex.StackTrace, false, true, 
                    type: Basics.Console.Type.Error);
            }
        }

        private void InvokePostExecution(object executeObject, string executionId, object result)
        {
            try
            {
                executeObject.GetType().GetMethod("PostExecute")?.Invoke(executeObject, new[] {executionId, result});
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                
                Basics.Console.Push(
                    "Domain PostExecution ERROR", ex.Message, ex.StackTrace, false, true, 
                    type: Basics.Console.Type.Error);
            }
        }

        private object GetCreateDomainInstance(Type domainInterface, out bool @new)
        {
            @new = false;
            
            lock (this._InstanceCreationLock)
            {
                if (this._ExecutableInstances.ContainsKey(domainInterface))
                    return this._ExecutableInstances[domainInterface];

                @new = true;

                Type interfaceType =
                    domainInterface.GetInterface("IDomainExecutable");

                if (interfaceType == null || !interfaceType.IsInterface ||
                    string.CompareOrdinal(interfaceType.FullName, "Xeora.Web.Basics.IDomainExecutable") != 0)
                    return new Exception("Calling Assembly is not a Xeora Domain Executable!");

                try
                {
                    this._ExecutableInstances[domainInterface] =
                        Activator.CreateInstance(domainInterface);
                }
                catch (Exception ex)
                {
                    return new Exception("Unable to create an instance of Xeora Domain Executable!", ex);
                }

                return this._ExecutableInstances[domainInterface];
            }
        }
        
        private object LoadDomainExecutable()
        {
            Type domainInterface =
                this._AssemblyDll.GetType($"Xeora.Domain.{this._ExecutableName}", false, true);
            
            if (domainInterface == null)
                return new Exception("Assembly does not belong to any Xeora Domain or Addon!");
            
            object executingDomain =
                this.GetCreateDomainInstance(domainInterface, out bool @new);
            if (!@new) return executingDomain;

            MethodInfo mI =
                executingDomain.GetType().GetMethod("Initialize");
            if (mI == null) return new MissingMethodException("Initialize");

            if (!domainInterface.Name.StartsWith("X") && domainInterface.Name.Length != 33)
            {
                Basics.Console.Push(
                    string.Empty,
                    $"Domain Executable: {domainInterface.Name} v{domainInterface.Assembly.GetName().Version}",
                    string.Empty, false);
            }

            try
            {
                mI.Invoke(executingDomain, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;

                Basics.Console.Push(
                    "Domain Initialization ERROR", ex.Message, ex.StackTrace, false, true,
                    type: Basics.Console.Type.Error);

                return new Exception("Xeora Domain executable could not be initialized!", ex);
            }
            
            return executingDomain;
        }

        private bool CheckFunctionResultTypeIsXeoraControl(Type methodReturnType) =>
            methodReturnType != null && this._XeoraControlTypes.ContainsKey(methodReturnType);

        private bool FixFunctionParameter(Type parameterType, ref object functionParam)
        {
            if (functionParam == null)
            {
                if (parameterType == typeof(byte) ||
                    parameterType == typeof(sbyte) ||
                    parameterType == typeof(short) ||
                    parameterType == typeof(ushort) ||
                    parameterType == typeof(int) ||
                    parameterType == typeof(uint) ||
                    parameterType == typeof(long) ||
                    parameterType == typeof(ulong) ||
                    parameterType == typeof(double) ||
                    parameterType == typeof(float))
                    functionParam = 0;

                return true;
            }

            if (ReferenceEquals(parameterType, functionParam.GetType()))
                return true;
            if (parameterType.IsInstanceOfType(functionParam))
                return true;
            if (string.Compare(parameterType.FullName, typeof(object).FullName, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            try
            {
                functionParam = 
                    Convert.ChangeType(functionParam, parameterType);
                return true;
            }
            catch (Exception)
            {
                if (!(functionParam is string param) || !string.IsNullOrEmpty(param)) return false;
                
                functionParam = null;
                return this.FixFunctionParameter(parameterType, ref functionParam);
            }
        }

        private MethodInfo GetAssemblyMethod(ref Type assemblyObject, Basics.Context.Request.HttpMethod httpMethod, string functionName, ref object[] functionParams, ExecuterTypes executerType)
        {
            MethodInfoFinder methodFinder =
                new MethodInfoFinder(httpMethod, functionName);
            string searchKey =
                $"{assemblyObject.FullName}.{methodFinder.Identifier}";

            MethodInfo[] possibleMethodInfos;
                
            lock (this._AssemblyMethodLock)
            {
                if (this._AssemblyMethods.ContainsKey(searchKey))
                    possibleMethodInfos = this._AssemblyMethods[searchKey];
                else
                {
                    possibleMethodInfos =
                        Array.FindAll(assemblyObject.GetMethods(), methodFinder.Find);
                    this._AssemblyMethods[searchKey] = possibleMethodInfos;
                }
            }

            MethodInfo methodInfoResult =
                this.FindBestMatch(possibleMethodInfos, ref functionParams, executerType);

            if (methodInfoResult != null)
                return methodInfoResult;

            if (assemblyObject.BaseType == null)
                return null;
            
            Type baseType = assemblyObject.BaseType;
            MethodInfo assemblyMethod =
                this.GetAssemblyMethod(ref baseType, httpMethod, functionName, ref functionParams, executerType);

            if (assemblyMethod != null)
                assemblyObject = baseType;

            return assemblyMethod;
        }

        private MethodInfo FindBestMatch(MethodInfo[] possibleMethodInfos, ref object[] functionParams, ExecuterTypes executerType)
        {
            MethodInfo worstMatch = null;

            foreach (MethodInfo methodInfo in possibleMethodInfos)
            {
                bool isXeoraControl =
                    this.CheckFunctionResultTypeIsXeoraControl(methodInfo.ReturnType);

                switch (executerType)
                {
                    case ExecuterTypes.Control:
                        if (!ReferenceEquals(methodInfo.ReturnType, typeof(object)) && !isXeoraControl)
                            continue;

                        break;
                    case ExecuterTypes.Other:
                        if (!isXeoraControl) break;
                        
                        // These are exceptional controls
                        if (methodInfo.ReturnType == typeof(Basics.ControlResult.RedirectOrder) ||
                            methodInfo.ReturnType == typeof(Basics.ControlResult.Message))
                            break;

                        continue;
                }

                ParameterInfo[] methodParams = 
                    methodInfo.GetParameters();

                switch (this.ExamParameters(methodParams, ref functionParams))
                {
                    case 0:
                        return methodInfo;
                    case 1:
                        worstMatch = methodInfo;
                        break;
                }
            }

            return worstMatch;
        }

        /// <summary>
        /// Exams the parameters.
        /// If returns -1, there is no match.
        /// If returns 0, it is the exact match.
        /// If returns 1, there can be better match.
        /// </summary>
        /// <returns>Match weight</returns>
        /// <param name="methodParams">Method parameters</param>
        /// <param name="functionParams">Pushed parameters</param>
        private int ExamParameters(ParameterInfo[] methodParams, ref object[] functionParams)
        {
            if (methodParams.Length == 0)
            {
                if (functionParams.Length == 0)
                    return 0;

                return -1;
            }

            if (methodParams.Length > functionParams.Length)
                return -1;

            object[] functionParamsReBuild =
                (object[])functionParams.Clone();

            for (int pC = 0; pC < methodParams.Length; pC++)
            {
                if (pC != methodParams.Length - 1)
                {
                    if (this.FixFunctionParameter(methodParams[pC].ParameterType, ref functionParamsReBuild[pC]))
                        continue;

                    return -1;
                }

                bool checkIsParamArrayDefined =
                    Attribute.IsDefined(methodParams[pC], typeof(ParamArrayAttribute));

                if (!checkIsParamArrayDefined)
                {
                    if (methodParams.Length != functionParamsReBuild.Length)
                        return -1;
                    
                    if (this.FixFunctionParameter(methodParams[pC].ParameterType, ref functionParamsReBuild[pC]))
                    {
                        functionParams = functionParamsReBuild;
                        return 0;
                    }

                    return -1;
                }

                Type paramArrayType = 
                    methodParams[pC].ParameterType.GetElementType();
                Array paramArrayValues =
                    Array.CreateInstance(paramArrayType, functionParamsReBuild.Length - methodParams.Length + 1);

                for (int pavC = 0; pC < functionParamsReBuild.Length; pavC++, pC++)
                {
                    this.FixFunctionParameter(paramArrayType, ref functionParamsReBuild[pC]);

                    paramArrayValues.SetValue(functionParamsReBuild[pC], pavC);
                }

                Array.Resize(ref functionParamsReBuild, methodParams.Length);
                functionParamsReBuild[methodParams.Length - 1] = paramArrayValues;

                functionParams = functionParamsReBuild;
                return 1;
            }

            return -1;
        }

        private Exception GetMethodException(Basics.Context.Request.HttpMethod httpMethod, string[] classNames, string functionName, object[] functionParams)
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
            foreach (object param in functionParams)
                sB.AppendFormat(", {0}", param.GetType());
            sB.AppendLine();
            sB.AppendFormat("HttpMethod: {0}", httpMethod);
            sB.AppendLine();

            return new TargetException(sB.ToString());
        }

        private object InvokeMethod(bool instanceExecute, Type executingDomain, MethodInfo assemblyMethod, object[] functionParams)
        {
            if (!instanceExecute)
                return assemblyMethod.Invoke(executingDomain, BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, functionParams, System.Globalization.CultureInfo.InvariantCulture);

            lock (this._InstanceCreationLock) 
            {
                if (!this._ExecutableInstances.ContainsKey(executingDomain))
                    throw new Exception("There is no Xeora Domain Executable instance has been created!");
                
                return assemblyMethod.Invoke(this._ExecutableInstances[executingDomain], BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, functionParams, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public object Invoke(Basics.Context.Request.HttpMethod httpMethod, string[] classNames, string functionName, object[] functionParams, bool instanceExecute, ExecuterTypes executerType)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentNullException(nameof(functionName));
            if (functionParams == null)
                functionParams = new object[] { };

            string executionId = 
                Guid.NewGuid().ToString();
            object result = null;

            object executeObject =
                this.LoadDomainExecutable();

            if (executeObject is Exception)
                return executeObject;

            try
            {
                Type assemblyObject = classNames != null 
                    ? this._AssemblyDll.GetType($"Xeora.Domain.{string.Join("+", classNames)}", true, true) 
                    : executeObject.GetType();

                MethodInfo assemblyMethod =
                    this.GetAssemblyMethod(ref assemblyObject, httpMethod, functionName, ref functionParams, executerType);

                if (assemblyMethod == null)
                    return this.GetMethodException(httpMethod, classNames, functionName, functionParams);

                this.InvokePreExecution(executeObject, executionId, assemblyMethod);

                result = this.InvokeMethod(instanceExecute, assemblyObject, assemblyMethod, functionParams);
            }
            catch (Exception ex)
            {
                return this.GetExecutionError(classNames, functionName, functionParams, ex);
            }
            finally
            {
                this.InvokePostExecution(executeObject, executionId, result);
            }
            
            return result;
        }

        public void Dispose()
        {
            Type domainInterface =
                this._AssemblyDll.GetType($"Xeora.Domain.{this._ExecutableName}", false, true);

            lock (this._InstanceCreationLock)
            {
                if (!this._ExecutableInstances.ContainsKey(domainInterface)) return;

                object executingDomain =
                    this._ExecutableInstances[domainInterface];
                
                try
                {
                    executingDomain.GetType().GetMethod("Terminate")?.Invoke(executingDomain, null);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex = ex.InnerException;

                    Basics.Console.Push(
                        "Domain Termination ERROR", ex.Message, ex.StackTrace, false, true,
                        type: Basics.Console.Type.Error);
                }
            }
        }
    }
}
