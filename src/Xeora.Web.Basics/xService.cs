using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Xml.XPath;

namespace Xeora.Web.Basics
{
    public class xService
    {
        public enum DataFlowTypes
        {
            Output,
            Input
        }

        public static event EventHandler<TransferProgressEventArgs> TransferProgress;
        public class TransferProgressEventArgs : EventArgs
        {
            public TransferProgressEventArgs(DataFlowTypes dataFlow, long current, long total)
            {
                this.DataFlow = dataFlow;
                this.Current = current;
                this.Total = total;
            }

            public DataFlowTypes DataFlow { get; private set; }
            public long Current { get; private set; }
            public long Total { get; private set; }
        }

        /// <summary>
        /// Authenticates to the Xeora xService
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceURL">xService URL</param>
        /// <param name="authenticationFunction">Authentication function</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="isAuthenticationDone">If set to <c>true</c> is authentication done</param>
        public static object AuthenticateToxService(string xServiceURL, string authenticationFunction, Parameters parameters, ref bool isAuthenticationDone)
        {
            object methodResult = 
                xService.CallxService(xServiceURL, authenticationFunction, parameters);

            isAuthenticationDone = !(methodResult != null && methodResult is Exception);

            if (isAuthenticationDone)
            {
                if (methodResult is ControlResult.Message && 
                    ((ControlResult.Message)methodResult).Type != ControlResult.Message.Types.Success)
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
        public static object CallxService(string xServiceURL, string functionName, Parameters parameters) =>
            xService.CallxService(xServiceURL, functionName, parameters, 60000);

        /// <summary>
        /// Calls the Xeora xService with timeout
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceURL">xService URL</param>
        /// <param name="functionName">Function name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="responseTimeout">Response timeout</param>
        public static object CallxService(string xServiceURL, string functionName, Parameters parameters, int responseTimeout)
        {
            HttpWebRequest httpWebRequest;
            HttpWebResponse httpWebResponse;

            Stream requestMS = null;
            try
            {
                requestMS = new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(
                        string.Format("xParams={0}", System.Web.HttpUtility.UrlEncode(parameters.ExecuteParametersXML))
                    )
                );
                requestMS.Seek(0, SeekOrigin.Begin);

                string responseString = null;
                string pageURL = 
                    string.Format("{0}?call={1}", xServiceURL, functionName);

                // Prepare Service Request Connection
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(pageURL);

                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = responseTimeout;
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.00; Windows 98)";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = requestMS.Length;
                // !--

                // Post ExecuteParametersXML to the Web Service
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

                        TransferProgress(null, new TransferProgressEventArgs(DataFlowTypes.Output, current, requestMS.Length));
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

                        TransferProgress(null, new TransferProgressEventArgs(DataFlowTypes.Input, current, httpWebResponse.ContentLength));
                    }
                } while (bC != 0);

                httpWebResponse.Close();
                GC.SuppressFinalize(httpWebResponse);

                return xService.ParsexServiceResult(responseString);
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

        private static object ParsexServiceResult(string resultXML)
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

                bool isDone = false;

                if (!xPathIter.MoveNext())
                    return new Exception("xService Response Error!");

                bool.TryParse(xPathIter.Current.GetAttribute("isdone", xPathIter.Current.NamespaceURI), out isDone);

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
                        ControlResult.Conditional.Conditions condition = 
                            ControlResult.Conditional.Conditions.Unknown;
                        System.Enum.TryParse<ControlResult.Conditional.Conditions>(xPathIter.Current.Value, out condition);

                        return new ControlResult.Conditional(condition);
                    case "Message":
                        ControlResult.Message.Types type =
                            ControlResult.Message.Types.Error;
                        System.Enum.TryParse<ControlResult.Message.Types>(xPathIter.Current.GetAttribute("messagetype", xPathIter.Current.NamespaceURI), out type);

                        return new ControlResult.Message(xPathIter.Current.Value, type);
                    case "ObjectFeed":
                        // TODO: Object Feed xService Implementation should be done!
                        return new Exception("Not Implemented Yet!");
                    case "PartialDataTable":
                        return xService.ParsePartialDataTable(ref xPathIter);
                    case "RedirectOrder":
                        return new ControlResult.RedirectOrder(xPathIter.Current.Value);
                    case "VariableBlock":
                        return xService.ParseVariableBlock(ref xPathIter);
                    default:
                        Type xTypeObject = xService.LoadTypeFromDomain(AppDomain.CurrentDomain, xType);

                        if (xTypeObject == null)
                            return xPathIter.Current.Value;

                        if (xService.SearchIsBaseType(xTypeObject, typeof(Exception)))
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

        private static ControlResult.PartialDataTable ParsePartialDataTable(ref XPathNodeIterator xPathIter)
        {
            ControlResult.PartialDataTable partialDataTable = 
                new ControlResult.PartialDataTable();

            int Total = 0;
            int.TryParse(xPathIter.Current.GetAttribute("total", xPathIter.Current.NamespaceURI), out Total);
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
                            xService.LoadTypeFromDomain(
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
                    new ControlResult.Message(
                        xPathIter_E.Current.Value.ToString(CultureInfo),
                        (ControlResult.Message.Types)System.Enum.Parse(
                            typeof(ControlResult.Message.Types),
                            xPathIter_E.Current.GetAttribute("messagetype", xPathIter_E.Current.NamespaceURI)
                        )
                    );
            }

            return partialDataTable;
        }

        private static ControlResult.VariableBlock ParseVariableBlock(ref XPathNodeIterator xPathIter)
        {
            ControlResult.VariableBlock variableBlock = 
                new ControlResult.VariableBlock();

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
                                xService.LoadTypeFromDomain(
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

        public class Parameters : Dictionary<string, string>
        {
            public Parameters() =>
                this.PublicKey = string.Empty;

            public Parameters(string executeParametersXML) : this() =>
                this.ParseExecuteParametersXML(executeParametersXML);

            public string ExecuteParametersXML => this.RenderExecuteParametersXML();

            private string RenderExecuteParametersXML()
            {
                StringWriter xmlStream = null;
                System.Xml.XmlTextWriter xmlWriter = null;
                try
                {
                    xmlStream = new StringWriter();
                    xmlWriter = new System.Xml.XmlTextWriter(xmlStream);

                    // Start Document Element
                    xmlWriter.WriteStartElement("ServiceParameters");

                    if (this.PublicKey != null)
                    {
                        xmlWriter.WriteStartElement("Item");
                        xmlWriter.WriteAttributeString("key", "PublicKey");
                        xmlWriter.WriteString(this.PublicKey);
                        xmlWriter.WriteEndElement();
                    }

                    Enumerator enumerator = this.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        xmlWriter.WriteStartElement("Item");
                        xmlWriter.WriteAttributeString("key", enumerator.Current.Key);
                        xmlWriter.WriteCData(enumerator.Current.Value);
                        xmlWriter.WriteEndElement();
                    }

                    // End Document Element
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();

                    return xmlStream.ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                finally
                {
                    if (xmlWriter != null)
                        xmlWriter.Close();

                    if (xmlStream != null)
                        xmlStream.Close();
                }
            }

            private void ParseExecuteParametersXML(string dataXML)
            {
                this.Clear();

                if (string.IsNullOrEmpty(dataXML))
                    return;

                StringReader xPathTextReader = null;
                try
                {
                    xPathTextReader = new StringReader(dataXML);
                    XPathDocument xPathDoc = new XPathDocument(xPathTextReader);

                    XPathNavigator xPathNavigator = xPathDoc.CreateNavigator();
                    XPathNodeIterator xPathIter = 
                        xPathNavigator.Select("/ServiceParameters/Item");

                    string key = null, value = null;
                    while (xPathIter.MoveNext())
                    {
                        key = xPathIter.Current.GetAttribute("key", xPathIter.Current.NamespaceURI);
                        value = xPathIter.Current.Value;

                        if (string.Compare(key, "PublicKey") == 0)
                        {
                            this.PublicKey = value;

                            continue;
                        }

                        base[key] = value;
                    }
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
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

            public string PublicKey { get; set; }
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
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormater.Binder = new OverrideBinder();

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
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormater.Binder = new OverrideBinder();

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
