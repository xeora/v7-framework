using System;
using System.Data;
using Xeora.Web.Manager.Execution;

namespace Xeora.Web.Manager
{
    public class Executer
    {
        // This function is for external call out side of the project DO NOT DISABLE IT
        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.Request.HttpMethod httpMethod, Basics.Execution.Bind bind) =>
            Executer.InvokeBind<T>(httpMethod, bind, ExecuterTypes.Undefined);

        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.Request.HttpMethod httpMethod, Basics.Execution.Bind bind, ExecuterTypes executerType)
        {
            if (bind == null)
                throw new NoNullAllowedException("Requires bind!");
            // Check if BindInfo Parameters has been parsed!
            if (!bind.Ready)
                throw new Exception("Bind Parameters should be parsed first!");

            Basics.Execution.InvokeResult<T> rInvokeResult =
                new Basics.Execution.InvokeResult<T>(bind);

            try
            {
                object invokedObject = 
                    ApplicationFactory.Prepare(bind.Executable).Invoke(
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
            Statement.Executable executableInfo =
                Statement.Factory.CreateExecutable(domainIdAccessTree, statementBlockId, statement, parameters != null && parameters.Length > 0, cache);

            if (executableInfo.Exception != null)
                return executableInfo.Exception;

            try
            {
                object invokedObject =
                    ApplicationFactory.Prepare(executableInfo.ExecutableName).Invoke(
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
