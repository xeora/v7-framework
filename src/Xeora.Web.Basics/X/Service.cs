using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Data;
using System.Xml.XPath;
using Xeora.Web.Basics.ControlResult;

namespace Xeora.Web.Basics.X
{
    public class Service
    {
        public delegate void TransferProgressHandler(long current, long total);
        public static event TransferProgressHandler InputTransferProgress;
        public static event TransferProgressHandler OutputTransferProgress;

        /// <summary>
        /// Authenticates to the Xeora xService
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceURL">xService URL</param>
        /// <param name="authenticationFunction">Authentication function</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="isAuthenticationDone">If set to <c>true</c> is authentication done</param>
        public static object Authenticate(string xServiceURL, string authenticationFunction, ServiceParameterCollection parameters, ref bool isAuthenticationDone)
        {
            object methodResult =
                Service.Call(xServiceURL, authenticationFunction, parameters);

            isAuthenticationDone = !(methodResult != null && methodResult is Exception);

            if (isAuthenticationDone)
            {
                if (methodResult is Message &&
                    ((Message)methodResult).Type != Message.Types.Success)
                    isAuthenticationDone = false;
            }

            return methodResult;
        }

        /// <summary>
        /// Calls the Xeora xService. Default timeout is 60 seconds
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceURL">xService URL</param>
        /// <param name="functionName">Function name</param>
        /// <param name="parameters">Parameters</param>
        public static object Call(string xServiceURL, string functionName, ServiceParameterCollection parameters) =>
            Service.Call(xServiceURL, functionName, parameters, 60000);

        /// <summary>
        /// Calls the Xeora xService with timeout
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceURL">xService URL</param>
        /// <param name="functionName">Function name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="responseTimeout">Response timeout in miliseconds</param>
        public static object Call(string xServiceURL, string functionName, ServiceParameterCollection parameters, int responseTimeout)
        {
            HttpWebRequest httpWebRequest;
            HttpWebResponse httpWebResponse;

            Stream requestMS = null;
            try
            {
                requestMS = new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(
                        string.Format("xParams={0}", System.Web.HttpUtility.UrlEncode(parameters.ToXML()))
                    )
                );
                requestMS.Seek(0, SeekOrigin.Begin);

                string responseString = null;
                string pageURL =
                    string.Format("{0}?call={1}", xServiceURL, functionName);

                // Prepare Service Request Connection
                httpWebRequest = (HttpWebRequest)WebRequest.Create(pageURL);

                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = responseTimeout;
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.00; Windows 98)";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = requestMS.Length;
                // !--

                // Post ServiceParametersXML to the Web Service
                byte[] buffer = new byte[512];
                int bC = 0;
                long current = 0;

                Stream transferStream = httpWebRequest.GetRequestStream();
                do
                {
                    bC = requestMS.Read(buffer, 0, buffer.Length);

                    if (bC > 0)
                    {
                        current += bC;

                        transferStream.Write(buffer, 0, bC);

                        OutputTransferProgress?.Invoke(current, requestMS.Length);
                    }
                } while (bC != 0);

                transferStream.Close();
                // !--

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                // Read and Parse Response Datas
                Stream resStream = httpWebResponse.GetResponseStream();

                responseString = string.Empty;
                current = 0;
                do
                {
                    bC = resStream.Read(buffer, 0, buffer.Length);

                    if (bC > 0)
                    {
                        current += bC;

                        responseString += System.Text.Encoding.UTF8.GetString(buffer, 0, bC);

                        InputTransferProgress?.Invoke(current, httpWebResponse.ContentLength);
                    }
                } while (bC != 0);

                httpWebResponse.Close();
                GC.SuppressFinalize(httpWebResponse);

                return Service.ParseServiceResult(responseString);
                // !--
            }
            catch (Exception ex)
            {
                return new Exception("xService Connection Error!", ex);
            }
            finally
            {
                if (requestMS != null)
                {
                    requestMS.Close();
                    GC.SuppressFinalize(requestMS);
                }
            }
        }

        private static object ParseServiceResult(string resultXML)
        {
            if (string.IsNullOrEmpty(resultXML))
                return new Exception("xService Response Error!");

            StringReader xPathTextReader = null;
            try
            {
                xPathTextReader = new StringReader(resultXML);
                XPathDocument xPathDoc = new XPathDocument(xPathTextReader);

                XPathNavigator xPathNavigator = xPathDoc.CreateNavigator();
                XPathNodeIterator xPathIter = xPathNavigator.Select("/ServiceResult");

                if (!xPathIter.MoveNext())
                    return new Exception("xService Response Error!");

                bool.TryParse(xPathIter.Current.GetAttribute("isdone", xPathIter.Current.NamespaceURI), out bool isDone);

                if (!isDone)
                    return new Exception("xService End-Point Process Error!");

                xPathIter = xPathNavigator.Select("/ServiceResult/Item");
                if (!xPathIter.MoveNext())
                    return null;

                string xType =
                    xPathIter.Current.GetAttribute("type", xPathIter.Current.NamespaceURI);

                if (string.IsNullOrEmpty(xType))
                    return null;

                switch (xType)
                {
                    case "Conditional":
                        Conditional.Conditions condition =
                            Conditional.Conditions.Unknown;
                        System.Enum.TryParse<Conditional.Conditions>(xPathIter.Current.Value, out condition);

                        return new Conditional(condition);
                    case "Message":
                        Message.Types type =
                            Message.Types.Error;
                        System.Enum.TryParse<Message.Types>(xPathIter.Current.GetAttribute("messagetype", xPathIter.Current.NamespaceURI), out type);

                        return new Message(xPathIter.Current.Value, type);
                    case "ObjectFeed":
                        // TODO: Object Feed xService Implementation should be done!
                        return new Exception("Not Implemented Yet!");
                    case "PartialDataTable":
                        return Service.ParsePartialDataTable(ref xPathIter);
                    case "RedirectOrder":
                        return new RedirectOrder(xPathIter.Current.Value);
                    case "VariableBlock":
                        return Service.ParseVariableBlock(ref xPathIter);
                    default:
                        Type xTypeObject = Service.LoadTypeFromDomain(AppDomain.CurrentDomain, xType);

                        if (xTypeObject == null)
                            return xPathIter.Current.Value;

                        if (Service.SearchIsBaseType(xTypeObject, typeof(Exception)))
                            return Activator.CreateInstance(xTypeObject, xPathIter.Current.Value, new Exception());

                        if (xTypeObject.IsPrimitive ||
                            object.ReferenceEquals(xTypeObject, typeof(short)) ||
                            object.ReferenceEquals(xTypeObject, typeof(int)) ||
                            object.ReferenceEquals(xTypeObject, typeof(long)))
                        {
                            return Convert.ChangeType(
                                xPathIter.Current.Value,
                                xTypeObject,
                                new System.Globalization.CultureInfo("en-US")
                            );
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(xPathIter.Current.Value))
                                return Serializer.Base64ToBinary(xPathIter.Current.Value);

                            return string.Empty;
                        }
                        catch (Exception ex)
                        {
                            return Activator.CreateInstance(ex.GetType(), ex.Message, new Exception());
                        }
                }
            }
            catch (Exception ex)
            {
                return new Exception("xService Response Error!", ex);
            }
            finally
            {
                if (xPathTextReader != null)
                {
                    xPathTextReader.Close();
                    GC.SuppressFinalize(xPathTextReader);
                }
            }
        }

        private static PartialDataTable ParsePartialDataTable(ref XPathNodeIterator xPathIter)
        {
            PartialDataTable partialDataTable =
                new PartialDataTable();

            int.TryParse(xPathIter.Current.GetAttribute("total", xPathIter.Current.NamespaceURI), out int Total);
            System.Globalization.CultureInfo CultureInfo =
                new System.Globalization.CultureInfo(xPathIter.Current.GetAttribute("cultureinfo", xPathIter.Current.NamespaceURI));

            partialDataTable.Locale = CultureInfo;
            partialDataTable.Total = Total;

            if (xPathIter.Current.MoveToFirstChild())
            {
                XPathNodeIterator xPathIter_C = xPathIter.Clone();

                if (xPathIter_C.Current.MoveToFirstChild())
                {
                    do
                    {
                        partialDataTable.Columns.Add(
                            xPathIter_C.Current.GetAttribute("name", xPathIter_C.Current.NamespaceURI),
                            Service.LoadTypeFromDomain(
                                AppDomain.CurrentDomain,
                                xPathIter_C.Current.GetAttribute("type", xPathIter_C.Current.NamespaceURI)
                            )
                        );
                    } while (xPathIter_C.Current.MoveToNext());
                }
            }

            if (xPathIter.Current.MoveToNext())
            {
                XPathNodeIterator xPathIter_R = xPathIter.Clone();

                if (xPathIter_R.Current.MoveToFirstChild())
                {
                    XPathNodeIterator xPathIter_RR;
                    DataRow tDR;

                    do
                    {
                        tDR = partialDataTable.NewRow();
                        xPathIter_RR = xPathIter_R.Clone();

                        if (xPathIter_RR.Current.MoveToFirstChild())
                        {
                            do
                            {
                                tDR[xPathIter_RR.Current.GetAttribute("name", xPathIter_RR.Current.NamespaceURI)] =
                                    xPathIter_RR.Current.Value.ToString(CultureInfo);
                            } while (xPathIter_RR.Current.MoveToNext());
                        }

                        partialDataTable.Rows.Add(tDR);
                    } while (xPathIter_R.Current.MoveToNext());
                }
            }

            if (xPathIter.Current.MoveToNext())
            {
                XPathNodeIterator xPathIter_E = xPathIter.Clone();

                partialDataTable.Message =
                    new Message(
                        xPathIter_E.Current.Value.ToString(CultureInfo),
                        (Message.Types)System.Enum.Parse(
                            typeof(Message.Types),
                            xPathIter_E.Current.GetAttribute("messagetype", xPathIter_E.Current.NamespaceURI)
                        )
                    );
            }

            return partialDataTable;
        }

        private static VariableBlock ParseVariableBlock(ref XPathNodeIterator xPathIter)
        {
            VariableBlock variableBlock =
                new VariableBlock();

            System.Globalization.CultureInfo CultureInfo =
                new System.Globalization.CultureInfo(xPathIter.Current.GetAttribute("cultureinfo", xPathIter.Current.NamespaceURI));

            if (xPathIter.Current.MoveToFirstChild())
            {
                XPathNodeIterator xPathIter_V = xPathIter.Clone();

                if (xPathIter_V.Current.MoveToFirstChild())
                {
                    do
                    {
                        variableBlock.Add(
                            xPathIter_V.Current.GetAttribute("key", xPathIter_V.Current.NamespaceURI),
                            Convert.ChangeType(
                                xPathIter_V.Current.Value.ToString(CultureInfo),
                                Service.LoadTypeFromDomain(
                                    AppDomain.CurrentDomain,
                                    xPathIter_V.Current.GetAttribute("type", xPathIter_V.Current.NamespaceURI)
                                )
                            )
                        );
                    } while (xPathIter_V.Current.MoveToNext());
                }
            }

            return variableBlock;
        }

        private static bool SearchIsBaseType(Type type, Type searchType)
        {
            do
            {
                if (object.ReferenceEquals(type, searchType))
                    return true;

                type = type.BaseType;
            } while (type != null && !object.ReferenceEquals(type, typeof(object)));

            return false;
        }

        private static Type LoadTypeFromDomain(AppDomain appDomain, string searchType)
        {
            Type typeResult = Type.GetType(searchType);

            if (typeResult != null)
                return typeResult;

            Assembly[] assms = appDomain.GetAssemblies();

            foreach (Assembly assm in assms)
            {
                typeResult = assm.GetType(searchType);

                if (typeResult != null)
                    return typeResult;
            }

            return null;
        }

        private class Serializer
        {
            public static string BinaryToBase64(object @object) =>
                Convert.ToBase64String(Serializer.Serialize(@object));

            public static byte[] Serialize(object @object)
            {
                if (@object == null)
                    return null;

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    {
                        Binder = new OverrideBinder()
                    };

                Stream serializationStream = null;
                try
                {
                    serializationStream = new MemoryStream();

                    binaryFormater.Serialize(serializationStream, @object);

                    byte[] result = new byte[serializationStream.Position];

                    serializationStream.Seek(0, SeekOrigin.Begin);
                    serializationStream.Read(result, 0, result.Length);

                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (serializationStream != null)
                    {
                        serializationStream.Close();
                        GC.SuppressFinalize(serializationStream);
                    }
                }
            }

            public static object Base64ToBinary(string serializedString) =>
                Serializer.Deserialize(Convert.FromBase64String(serializedString));

            public static object Deserialize(byte[] serializedBytes)
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    {
                        Binder = new OverrideBinder()
                    };

                Stream serializationStream = null;
                try
                {
                    serializationStream = new MemoryStream(serializedBytes);

                    return binaryFormater.Deserialize(serializationStream);
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    if (serializationStream != null)
                    {
                        serializationStream.Close();
                        GC.SuppressFinalize(serializationStream);
                    }
                }
            }
        }
    }
}
