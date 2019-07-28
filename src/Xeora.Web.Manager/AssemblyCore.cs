using System;
using System.IO;
using System.IO.Compression;
using System.Data;

namespace Xeora.Web.Manager
{
    public class AssemblyCore
    {
        public static string EncodeFunction(string encodingHashCode, string bindFunctionForEncoding)
        {
            string[] funcParts = 
                bindFunctionForEncoding.Split(',');

            // First Part of Encoded Bind Function
            //   [AssemblyName]?[ClassName].[FunctionName]
            string executionPart = funcParts[0];
            // Second Part of Encoded Bind Function (Parameters)
            //   [Parameter]|[Parameter]|...
            string parameterPart = null;

            if (funcParts.Length == 2)
                parameterPart = funcParts[1];

            // FP will be Encoded with Base64
            //   Base64 contains encrypted Data
            //   EncData = XOR HashCode applied on to deflated compression
            string encodedExecutionPart;
            // Parameters will be Encoded with Base64
            string encodedParameterPart = string.Empty;

            if (!string.IsNullOrEmpty(parameterPart))
                encodedParameterPart = 
                    Tools.Serialization.Base64.Serialize(
                        System.Text.Encoding.UTF8.GetBytes(parameterPart));

            Stream compressedStream = null;
            GZipStream gzipStream = null;
            try
            {
                byte[] buffer =
                    System.Text.Encoding.UTF8.GetBytes(executionPart);

                compressedStream = new MemoryStream();
                try
                {
                    gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true);

                    gzipStream.Write(buffer, 0, buffer.Length);
                    gzipStream.Flush();
                }
                finally
                {
                    gzipStream?.Close();
                }

                compressedStream.Seek(0, SeekOrigin.Begin);

                // EncData Dec. Process
                while (compressedStream.Position != compressedStream.Length)
                {
                    byte byteCoded = 
                        (byte)compressedStream.ReadByte();
                    byteCoded = 
                        (byte)(byteCoded ^ 
                           Convert.ToByte(
                               encodingHashCode[(int)(compressedStream.Position - 1) % encodingHashCode.Length]
                            )
                        );

                    compressedStream.Seek(-1, SeekOrigin.Current);
                    compressedStream.WriteByte(byteCoded);
                }
                // !--

                buffer = new byte[compressedStream.Length];

                compressedStream.Seek(0, SeekOrigin.Begin);
                compressedStream.Read(buffer, 0, buffer.Length);

                encodedExecutionPart = 
                    Tools.Serialization.Base64.Serialize(buffer);
            }
            finally
            {
                compressedStream?.Close();
            }

            string encodedBindFunction =
                $"{encodingHashCode},{System.Web.HttpUtility.UrlEncode(encodedExecutionPart)}";

            if (parameterPart != null)
                encodedBindFunction = $"{encodedBindFunction},{System.Web.HttpUtility.UrlEncode(encodedParameterPart)}";

            return encodedBindFunction;
        }

        public static string DecodeFunction(string encodedBindFunction)
        {
            string[] bindParts = encodedBindFunction.Split(',');

            string encodingHashCode = bindParts[0];
            // First Part of Encoded Bind Function
            //   [AssemblyName]?[ClassName].[FunctionName]
            // FP is Encoded with Base64
            //   Base64 contains encrypted Data
            //   EncData = XOR HashCode applied on to deflated compression
            string encodedExecutionPart = bindParts[1];
            if (!encodedExecutionPart.Contains("+") && encodedExecutionPart.Contains("%"))
                encodedExecutionPart = System.Web.HttpUtility.UrlDecode(encodedExecutionPart);
            // Second Part of Encoded Call Function (Parameters)
            //   [Parameter]|[Parameter]|...
            // Parameters are Encoded with Base64
            string encodedParameterPart = string.Empty;
            if (bindParts.Length == 3)
            {
                encodedParameterPart = bindParts[2];

                if (!encodedParameterPart.Contains("+") && encodedParameterPart.Contains("%"))
                    encodedParameterPart = System.Web.HttpUtility.UrlDecode(encodedParameterPart);
            }

            System.Text.StringBuilder encodedText =
                new System.Text.StringBuilder();
            
            byte[] buffer = 
                Tools.Serialization.Base64.DeSerialize(encodedExecutionPart);

            // EncData to DecData Process
            for (int bC = 0; bC < buffer.Length; bC++)
                buffer[bC] = (byte)(buffer[bC] ^ Convert.ToByte(encodingHashCode[bC % encodingHashCode.Length]));
            // !--

            Stream compressedStream = null;
            GZipStream gzipStream = null;
            try
            {
                // Prepare Content Stream
                compressedStream = new MemoryStream(buffer);
                compressedStream.Seek(0, SeekOrigin.Begin);
                // !--
                
                buffer = new byte[512];
                gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress, true);
                do
                {
                    int bC = gzipStream.Read(buffer, 0, buffer.Length);
                    if (bC == 0) break;
                    
                    encodedText.Append(System.Text.Encoding.UTF8.GetString(buffer, 0, bC));
                } while(true);
            }
            finally
            {
                gzipStream?.Close();
                compressedStream?.Close();
            }
            string decodedExecutionPart = 
                encodedText.ToString();

            // Decode The Parameters Part
            if (string.IsNullOrEmpty(encodedParameterPart)) return decodedExecutionPart;
            
            string decodedParameterPart = 
                System.Text.Encoding.UTF8.GetString(
                    Tools.Serialization.Base64.DeSerialize(encodedParameterPart));

            return $"{decodedExecutionPart},{decodedParameterPart}";
        }

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
