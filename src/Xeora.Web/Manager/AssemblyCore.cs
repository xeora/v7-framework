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
            string[] splitEF = bindFunctionForEncoding.Split(',');

            // First Part of Encoded Bind Function
            //   [AssemblyName]?[ClassName].[FunctionName]
            string decodedBF01 = splitEF[0];
            // Second Part of Encoded Bind Function (Parameters)
            //   [Parameter]|[Parameter]|...
            bool isDecodedBF02Exist = false;
            string decodedBF02 = string.Empty;

            if (splitEF.Length == 2)
            {
                decodedBF02 = splitEF[1];
                isDecodedBF02Exist = true;
            }

            // FP will be Encoded with Base64
            //   Base64 contains encrypted Data
            //   EncData = XOR HashCode applied on to deflated compression
            string encodedCF01 = string.Empty;
            // Parameters will be Encoded with Base64
            string encodedCF02 = string.Empty;

            if (!string.IsNullOrEmpty(decodedBF02))
                encodedCF02 = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(decodedBF02));

            Stream gzipHelperStream = null;
            GZipStream gzipStream = null;
            try
            {
                byte[] rbuffer =
                    System.Text.Encoding.UTF8.GetBytes(decodedBF01);

                gzipHelperStream = new MemoryStream();
                try
                {
                    gzipStream = new GZipStream(gzipHelperStream, CompressionMode.Compress, true);

                    gzipStream.Write(rbuffer, 0, rbuffer.Length);
                    gzipStream.Flush();
                }
                finally
                {
                    if (gzipStream != null)
                    {
                        gzipStream.Close();
                        GC.SuppressFinalize(gzipStream);
                    }
                }

                gzipHelperStream.Seek(0, SeekOrigin.Begin);

                // EncData Dec. Process
                byte byteCoded = 0;
                int bC = 0;
                while (gzipHelperStream.Position != gzipHelperStream.Length)
                {
                    byteCoded = (byte)gzipHelperStream.ReadByte();
                    byteCoded = (byte)(byteCoded ^ Convert.ToByte(encodingHashCode[bC % encodingHashCode.Length]));

                    gzipHelperStream.Seek(-1, SeekOrigin.Current);
                    gzipHelperStream.WriteByte(byteCoded);

                    bC += 1;
                }
                // !--

                rbuffer = new byte[gzipHelperStream.Length];

                gzipHelperStream.Seek(0, SeekOrigin.Begin);
                gzipHelperStream.Read(rbuffer, 0, rbuffer.Length);

                encodedCF01 = Convert.ToBase64String(rbuffer);
            }
            finally
            {
                if (gzipHelperStream != null)
                {
                    gzipHelperStream.Close();
                    GC.SuppressFinalize(gzipHelperStream);
                }
            }

            string encodedBindFunction =
                string.Format("{0},{1}", encodingHashCode, System.Web.HttpUtility.UrlEncode(encodedCF01));

            if (isDecodedBF02Exist)
                encodedBindFunction = string.Format("{0},{1}", encodedBindFunction, System.Web.HttpUtility.UrlEncode(encodedCF02));

            return encodedBindFunction;
        }

        public static string DecodeFunction(string encodedBindFunction)
        {
            string[] splitEF = encodedBindFunction.Split(',');

            string decodedBF01 = string.Empty;
            string decodedBF02 = null;
            string encodingHashCode = splitEF[0];
            // First Part of Encoded Bind Function
            //   [AssemblyName]?[ClassName].[FunctionName]
            // FP is Encoded with Base64
            //   Base64 contains encrypted Data
            //   EncData = XOR HashCode applied on to deflated compression
            string encodedBF01 = splitEF[1];
            if (!encodedBF01.Contains("+") && encodedBF01.Contains("%"))
                encodedBF01 = System.Web.HttpUtility.UrlDecode(encodedBF01);
            // Second Part of Encoded Call Function (Parameters)
            //   [Parameter]|[Parameter]|...
            // Parameters are Encoded with Base64
            string encodedBF02 = string.Empty;
            if (splitEF.Length == 3)
            {
                encodedBF02 = splitEF[2];

                if (!encodedBF02.Contains("+") && encodedBF02.Contains("%"))
                    encodedBF02 = System.Web.HttpUtility.UrlDecode(encodedBF02);
            }

            System.Text.StringBuilder encodedText =
                new System.Text.StringBuilder();

            int bC = 0;
            byte[] buffer = Convert.FromBase64String(encodedBF01);

            // EncData to DecData Process
            for (bC = 0; bC < buffer.Length; bC++)
                buffer[bC] = (byte)(buffer[bC] ^ Convert.ToByte(encodingHashCode[bC % encodingHashCode.Length]));
            // !--

            Stream gzipHelperStream = null;
            GZipStream gzipStream = null;
            try
            {
                byte[] rbuffer = new byte[512];

                // Prepare Content Stream
                gzipHelperStream = new MemoryStream();
                gzipHelperStream.Write(buffer, 0, buffer.Length);
                gzipHelperStream.Flush();
                gzipHelperStream.Seek(0, SeekOrigin.Begin);
                // !--

                gzipStream = new GZipStream(gzipHelperStream, CompressionMode.Decompress, true);
                do
                {
                    bC = gzipStream.Read(rbuffer, 0, rbuffer.Length);

                    if (bC > 0)
                        encodedText.Append(System.Text.Encoding.UTF8.GetString(rbuffer, 0, bC));
                } while (bC > 0);
            }
            finally
            {
                if (gzipStream != null)
                {
                    gzipStream.Close();
                    GC.SuppressFinalize(gzipStream);
                }

                if (gzipHelperStream != null)
                {
                    gzipHelperStream.Close();
                    GC.SuppressFinalize(gzipHelperStream);
                }
            }

            decodedBF01 = encodedText.ToString();

            // Decode The Parameters Part
            if (!string.IsNullOrEmpty(encodedBF02))
            {
                decodedBF02 = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(encodedBF02));

                return string.Format("{0},{1}", decodedBF01, decodedBF02);
            }

            return decodedBF01;
        }

        // This function is for external call out side of the project DO NOT DISABLE IT
        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.HttpMethod httpMethod, Basics.Execution.Bind bind) =>
            AssemblyCore.InvokeBind<T>(httpMethod, bind, ExecuterTypes.Undefined);

        public static Basics.Execution.InvokeResult<T> InvokeBind<T>(Basics.Context.HttpMethod httpMethod, Basics.Execution.Bind bind, ExecuterTypes executerType)
        {
            if (bind == null)
                throw new NoNullAllowedException("Requires bind!");
            // Check if BindInfo Parameters has been parsed!
            if (!bind.Ready)
                throw new System.Exception("Bind Parameters shoud be parsed first!");

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

                if (invokedObject is System.Exception)
                    throw (System.Exception)invokedObject;

                rInvokeResult.Result = (T)invokedObject;
            }
            catch (System.Exception ex)
            {
                Helper.EventLogger.Log(ex);

                rInvokeResult.Exception = ex;
            }

            return rInvokeResult;
        }

        public static object ExecuteStatement(string[] domainIdAccessTree, string statementBlockId, string statement, object[] parameters, bool cache)
        {
            StatementExecutable executableInfo =
                StatementFactory.CreateExecutable(domainIdAccessTree, statementBlockId, statement, (parameters != null && parameters.Length > 0), cache);

            if (executableInfo.Exception != null)
                return executableInfo.Exception;

            Master.Initialize();

            try
            {
                object invokedObject =
                    Application.Prepare(executableInfo.ExecutableName).Invoke(
                        Basics.Context.HttpMethod.GET,
                        new string[] { executableInfo.ClassName },
                        "Execute",
                        parameters,
                        false,
                        ExecuterTypes.Undefined
                    );

                if (invokedObject is System.Exception)
                    throw (System.Exception)invokedObject;

                return invokedObject;
            }
            catch (System.Exception ex)
            {
                Helper.EventLogger.Log(ex);

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
