using System;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Threading;

namespace Xeora.Web.Manager
{
    public class AssemblyCore
    {
        // This function is for external call out side of the project DO NOT DISABLE IT
        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.Request.HttpMethod httpMethod, Basics.Execution.Bind bind) =>
            AssemblyCore.InvokeBind<T>(httpMethod, bind, ExecuterTypes.Undefined);

        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.Request.HttpMethod httpMethod, Basics.Execution.Bind bind, ExecuterTypes executerType)
        {
            if (bind == null)
                throw new NoNullAllowedException("Requires bind!");
            // Check if BindInfo Parameters has been parsed!
            if (!bind.Ready)
                throw new Exception("Bind Parameters should be parsed first!");

            Master.Initialize();

            Basics.Execution.InvokeResult<T> rInvokeResult =
                new Basics.Execution.InvokeResult<T>(bind);

            try
            {
                object invokedObject = 
                    Application.Prepare(bind.Executable).Invoke(
                        httpMethod,
                        bind.Classes,
                        bind.Procedure,
                        bind.Parameters.Values,
                        bind.InstanceExecution,
                        executerType
                    );

                if (invokedObject is Exception exception)
                    throw exception;

                rInvokeResult.Result = (T)invokedObject;
            }
            catch (Exception ex)
            {
                Tools.EventLogger.Log(ex);

                rInvokeResult.Exception = ex;
            }

            return rInvokeResult;
        }

        public static object ExecuteStatement(string[] domainIdAccessTree, string statementBlockId, string statement, object[] parameters, bool cache)
        {
            StatementExecutable executableInfo =
                StatementFactory.CreateExecutable(domainIdAccessTree, statementBlockId, statement, parameters != null && parameters.Length > 0, cache);

            if (executableInfo.Exception != null)
                return executableInfo.Exception;

            Master.Initialize();

            try
            {
                object invokedObject =
                    Application.Prepare(executableInfo.ExecutableName).Invoke(
                        Basics.Context.Request.HttpMethod.GET,
                        new string[] { executableInfo.ClassName },
                        "Execute",
                        parameters,
                        false,
                        ExecuterTypes.Undefined
                    );

                if (invokedObject is Exception exception)
                    throw exception;

                return invokedObject;
            }
            catch (Exception ex)
            {
                Tools.EventLogger.Log(ex);

                return ex;
            }
        }

        public static string GetPrimitiveValue(object methodResult)
        {
            if (methodResult != null &&
                (methodResult.GetType().IsPrimitive || methodResult is string))
                return (string)methodResult;

            return null;
        }
    }
}
