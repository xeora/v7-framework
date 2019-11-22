using System;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Xeora.Web.Basics;

namespace Xeora.Web.Manager.Execution
{
    internal class ApplicationContext : AssemblyLoadContext
    {
        private readonly INegotiator _Negotiator;
        private readonly string _ExecutablePath;
        private readonly string _ExecutableName;
        
        private readonly AssemblyDependencyResolver _DependencyResolver;
        private Assembly _AssemblyDll;

        private readonly object _InstanceCreationLock;
        private readonly ConcurrentDictionary<Type, DomainExecutable> _ExecutableInstances;
        
        private readonly object _AssemblyMethodLock;
        private readonly Dictionary<string, MethodInfo[]> _AssemblyMethods;

        public ApplicationContext(INegotiator negotiator, string executablesPath, string executableName)
        {
            this._Negotiator = negotiator;

            if (string.IsNullOrEmpty(executablesPath))
                throw new ArgumentNullException(nameof(executablesPath));
            if (string.IsNullOrEmpty(executableName))
                throw new ArgumentNullException(nameof(executableName));

            this.Resolving += this.ResolveAssembly;

            this._ExecutableName = executableName;
            this._ExecutablePath =
                Path.Combine(executablesPath, $"{this._ExecutableName}.dll");
            this._DependencyResolver = new AssemblyDependencyResolver(this._ExecutablePath);
                
            this._InstanceCreationLock = new object();
            this._ExecutableInstances = 
                new ConcurrentDictionary<Type, DomainExecutable>();
            
            this._AssemblyMethodLock = new object();
            this._AssemblyMethods = 
                new Dictionary<string, MethodInfo[]>();
        }

        private Assembly ResolveAssembly(AssemblyLoadContext sender, AssemblyName assemblyName)
        {
            if (assemblyName.FullName.IndexOf("Xeora.Web.Basics,", StringComparison.Ordinal) == 0)
                return null;
            
            string assemblyPath = 
                this._DependencyResolver.ResolveAssemblyToPath(assemblyName);

            return !string.IsNullOrEmpty(assemblyPath) ? sender.LoadFromAssemblyPath(assemblyPath) : null;
        }

        protected override Assembly Load(AssemblyName assemblyName) =>
            this.ResolveAssembly(this, assemblyName);

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

        public bool MissingFileException { get; private set; }

        private Exception GetExecutionError(string[] classNames, string functionName, IReadOnlyCollection<object> functionParams, Exception innerException)
        {
            string compileErrorObject;

            if (classNames != null)
            {
                compileErrorObject = functionParams.Count == 0 
                    ? $"{this._ExecutableName}?{string.Join(".", classNames)}.{functionName}"
                    : $"{this._ExecutableName}?{string.Join(".", classNames)}.{functionName},[Length:{functionParams.Count}]";
            }
            else
            {
                compileErrorObject = functionParams.Count == 0 
                    ? $"{this._ExecutableName}?{functionName}"
                    : $"{this._ExecutableName}?{functionName},[Length:{functionParams.Count}]";
            }

            return new Exception(
                $"Executable Execution Error! RequestInfo: {compileErrorObject}", innerException);
        }

        private void InvokePreExecution(DomainExecutable domainInstance, string executionId, MethodInfo assemblyMethod)
        {
            try
            {
                domainInstance.PreExecute(executionId, assemblyMethod);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                
                Basics.Console.Push(
                    "Execution Exception...", ex.Message, ex.StackTrace, false, true, 
                    type: Basics.Console.Type.Error);
            }
        }

        private void InvokePostExecution(DomainExecutable domainInstance, string executionId, ref object result)
        {
            try
            {
                domainInstance.PostExecute(executionId, ref result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                
                Basics.Console.Push(
                    "Execution Exception...", ex.Message, ex.StackTrace, false, true, 
                    type: Basics.Console.Type.Error);
            }
        }

        private DomainExecutable GetCreateDomainInstance(Type executingDomain, out Exception exception)
        {
            exception = null;
            
            Monitor.Enter(this._InstanceCreationLock);
            try
            {
                if (this._ExecutableInstances.TryGetValue(executingDomain, out DomainExecutable domainInstance))
                    return domainInstance;
                
                if (executingDomain.BaseType == null || executingDomain.BaseType != typeof(DomainExecutable))
                {
                    exception = new Exception("Calling Assembly is not a Xeora Domain Executable!");
                    return null;
                }

                try
                {
                    domainInstance = 
                        (DomainExecutable)Activator.CreateInstance(executingDomain, new DomainPacket(this.Name, this._Negotiator));
                    this._ExecutableInstances.TryAdd(executingDomain, domainInstance);
                    
                    if (!executingDomain.Name.StartsWith("X") && executingDomain.Name.Length != 33)
                    {
                        Basics.Console.Push(
                            string.Empty,
                            $"Initialized Executable: {executingDomain.Name} v{executingDomain.Assembly.GetName().Version}",
                            string.Empty, false, true);
                    }

                    return domainInstance;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex = ex.InnerException;

                    Basics.Console.Push(
                        "Execution Exception...", ex.Message, ex.StackTrace, false, true,
                        type: Basics.Console.Type.Error);

                    exception = new Exception("Xeora Domain executable could not be initialized!", ex);
                    return null;
                }
            }
            finally
            {
                Monitor.Exit(this._InstanceCreationLock);
            }
        }

        private DomainExecutable LoadDomainExecutable(out Exception exception)
        {
            Type executingDomain =
                this._AssemblyDll.GetType($"Xeora.Domain.{this._ExecutableName}", false, true);

            if (executingDomain != null) return this.GetCreateDomainInstance(executingDomain, out exception);
            
            exception = new Exception("Assembly does not belong to any Xeora Domain or Addon!");
            return null;
        }

        private bool CheckFunctionResultTypeIsXeoraControl(Type methodReturnType)
        {
            if (methodReturnType == null) return false;
            
            if (methodReturnType == typeof(Basics.ControlResult.Conditional)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.Message)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.ObjectFeed)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.RedirectOrder)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.VariableBlock)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.DirectDataAccess)) return true;
            if (methodReturnType == typeof(Basics.ControlResult.PartialDataTable)) return true;

            return false;
        }

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

                    if (!this.FixFunctionParameter(methodParams[pC].ParameterType, ref functionParamsReBuild[pC]))
                        return -1;
                    
                    functionParams = functionParamsReBuild;
                    return 0;
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

            if (!this._ExecutableInstances.TryGetValue(executingDomain, out DomainExecutable domainInstance))
                throw new Exception("There is no Xeora Domain Executable instance has been created!");

            return assemblyMethod.Invoke(domainInstance,
                BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, functionParams,
                System.Globalization.CultureInfo.InvariantCulture);
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

            DomainExecutable domainInstance =
                this.LoadDomainExecutable(out Exception exception);
            if (exception != null) return exception;

            try
            {
                Type assemblyObject = classNames != null 
                    ? this._AssemblyDll.GetType($"Xeora.Domain.{string.Join("+", classNames)}", true, true) 
                    : domainInstance.GetType();

                MethodInfo assemblyMethod =
                    this.GetAssemblyMethod(ref assemblyObject, httpMethod, functionName, ref functionParams, executerType);

                if (assemblyMethod == null)
                    return this.GetMethodException(httpMethod, classNames, functionName, functionParams);

                this.InvokePreExecution(domainInstance, executionId, assemblyMethod);

                result = this.InvokeMethod(instanceExecute, assemblyObject, assemblyMethod, functionParams);
            }
            catch (Exception ex)
            {
                return this.GetExecutionError(classNames, functionName, functionParams, ex);
            }
            finally
            {
                this.InvokePostExecution(domainInstance, executionId, ref result);
            }
            
            return result;
        }

        public void Terminate()
        {
            Type executingDomain =
                this._AssemblyDll.GetType($"Xeora.Domain.{this._ExecutableName}", false, true);

            if (!Monitor.TryEnter(this._InstanceCreationLock))
                return;
            
            try
            {
                if (!this._ExecutableInstances.TryRemove(executingDomain, out DomainExecutable domainInstance)) return;
                
                try
                {
                    domainInstance.Terminate();;
                    
                    if (!executingDomain.Name.StartsWith("X") && executingDomain.Name.Length != 33)
                    {
                        Basics.Console.Push(
                            string.Empty,
                            $"Terminated Executable: {executingDomain.Name} v{executingDomain.Assembly.GetName().Version}",
                            string.Empty, false, true);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex = ex.InnerException;

                    Basics.Console.Push(
                        "Execution Exception...", ex.Message, ex.StackTrace, false, true,
                        type: Basics.Console.Type.Error);
                }
            }
            finally
            {
                Monitor.Exit(this._InstanceCreationLock);
            }
        }
    }
}
