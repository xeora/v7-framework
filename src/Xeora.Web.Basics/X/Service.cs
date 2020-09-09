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
        /// <param name="xServiceUrl">xService Url</param>
        /// <param name="authenticationFunction">Authentication function</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="isAuthenticationDone">If set to <c>true</c> is authentication done</param>
        public static object Authenticate(string xServiceUrl, string authenticationFunction, ServiceParameterCollection parameters, ref bool isAuthenticationDone)
        {
            object methodResult =
                Service.Call(xServiceUrl, authenticationFunction, parameters);

            isAuthenticationDone = !(methodResult is Exception);

            if (!isAuthenticationDone) return methodResult;
            
            if (methodResult is Message message &&
                message.Type != Message.Types.Success)
                isAuthenticationDone = false;

            return methodResult;
        }

        /// <summary>
        /// Calls the Xeora xService. Default timeout is 60 seconds
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceUrl">xService Url</param>
        /// <param name="functionName">Function name</param>
        /// <param name="parameters">Parameters</param>
        public static object Call(string xServiceUrl, string functionName, ServiceParameterCollection parameters) =>
            Service.Call(xServiceUrl, functionName, parameters, 60000);

        /// <summary>
        /// Calls the Xeora xService with timeout
        /// </summary>
        /// <returns>The result of xService</returns>
        /// <param name="xServiceUrl">xService Url</param>
        /// <param name="functionName">Function name</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="responseTimeout">Response timeout in milliseconds</param>
        public static object Call(string xServiceUrl, string functionName, ServiceParameterCollection parameters, int responseTimeout)
        {
            Stream requestStream = null;
            try
            {
                requestStream = new MemoryStream(
                    System.Text.Encoding.UTF8.GetBytes(
                        $"xParams={System.Web.HttpUtility.UrlEncode(parameters.ToXml())}"
                    )
                );
                requestStream.Seek(0, SeekOrigin.Begin);

                string responseString = null;
                string pageUrl =
                    $"{xServiceUrl}?call={functionName}";

                // Prepare Service Request Connection
                HttpWebRequest httpWebRequest = 
                    (HttpWebRequest)WebRequest.Create(pageUrl);

                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = responseTimeout;
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.00; Windows 98)";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = requestStream.Length;
                // !--

                // Post ServiceParametersXML to the Web Service
                byte[] buffer = new byte[512];
                int bC;
                long current = 0;

                Stream transferStream = httpWebRequest.GetRequestStream();
                do
                {
                    bC = requestStream.Read(buffer, 0, buffer.Length);

                    if (bC <= 0) continue;
                    
                    current += bC;

                    transferStream.Write(buffer, 0, bC);

                    OutputTransferProgress?.Invoke(current, requestStream.Length);
                } while (bC != 0);

                transferStream.Close();
                // !--

                HttpWebResponse httpWebResponse = 
                    (HttpWebResponse)httpWebRequest.GetResponse();

                // Read and Parse Response Datas
                Stream resStream = httpWebResponse.GetResponseStream();

                responseString = string.Empty;
                current = 0;
                do
                {
                    bC = resStream.Read(buffer, 0, buffer.Length);

                    if (bC <= 0) continue;
                    
                    current += bC;

                    responseString += System.Text.Encoding.UTF8.GetString(buffer, 0, bC);

                    InputTransferProgress?.Invoke(current, httpWebResponse.ContentLength);
                } while (bC != 0);

                httpWebResponse.Close();

                return Service.ParseServiceResult(responseString);
                // !--
            }
            catch (Exception ex)
            {
                return new Exception("xService Connection Error!", ex);
            }
            finally
            {
                requestStream?.Close();
            }
        }

        private static object ParseServiceResult(string resultXml)
        {
            if (string.IsNullOrEmpty(resultXml))
                return new Exception("xService Response Error!");

            StringReader xPathTextReader = null;
            try
            {
                xPathTextReader = new StringReader(resultXml);
                XPathDocument xPathDoc = new XPathDocument(xPathTextReader);

                XPathNavigator xPathNavigator = xPathDoc.CreateNavigator();
                XPathNodeIterator xPathIter = xPathNavigator.Select("/ServiceResult");

                if (!xPathIter.MoveNext())
                    return new Exception("xService Response Error!");

                bool.TryParse(xPathIter.Current?.GetAttribute("isdone", xPathIter.Current.NamespaceURI), out bool isDone);

                if (!isDone)
                    return new Exception("xService End-Point Process Error!");

                xPathIter = xPathNavigator.Select("/ServiceResult/Item");
                if (!xPathIter.MoveNext())
                    return null;

                string xType =
                    xPathIter.Current?.GetAttribute("type", xPathIter.Current.NamespaceURI);

                if (string.IsNullOrEmpty(xType))
                    return null;

                switch (xType)
                {
                    case "Conditional":
                        System.Enum.TryParse(xPathIter.Current.Value, out Conditional.Conditions condition);

                        return new Conditional(condition);
                    case "Message":
                        System.Enum.TryParse(xPathIter.Current.GetAttribute("messagetype", xPathIter.Current.NamespaceURI), out Message.Types type);

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
                            ReferenceEquals(xTypeObject, typeof(short)) ||
                            ReferenceEquals(xTypeObject, typeof(int)) ||
                            ReferenceEquals(xTypeObject, typeof(long)))
                        {
                            return Convert.ChangeType(
                                xPathIter.Current.Value,
                                xTypeObject,
                                System.Globalization.CultureInfo.InvariantCulture
                            );
                        }

                        try
                        {
                            return !string.IsNullOrEmpty(xPathIter.Current.Value) ? Serializer.Base64ToBinary(xPathIter.Current.Value) : string.Empty;
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
                xPathTextReader?.Close();
            }
        }

        private static PartialDataTable ParsePartialDataTable(ref XPathNodeIterator xPathIter)
        {
            PartialDataTable partialDataTable =
                new PartialDataTable();

            int.TryParse(xPathIter.Current?.GetAttribute("total", xPathIter.Current.NamespaceURI), out int total);
            System.Globalization.CultureInfo cultureInfo =
                new System.Globalization.CultureInfo(xPathIter.Current?.GetAttribute("cultureinfo", xPathIter.Current.NamespaceURI));

            partialDataTable.Locale = cultureInfo;
            partialDataTable.Total = total;

            if (xPathIter.Current != null && xPathIter.Current.MoveToFirstChild())
            {
                XPathNodeIterator xPathIterC = xPathIter.Clone();

                if (xPathIterC.Current != null && xPathIterC.Current.MoveToFirstChild())
                {
                    do
                    {
                        partialDataTable.Columns.Add(
                            xPathIterC.Current.GetAttribute("name", xPathIterC.Current.NamespaceURI),
                            Service.LoadTypeFromDomain(
                                AppDomain.CurrentDomain,
                                xPathIterC.Current.GetAttribute("type", xPathIterC.Current.NamespaceURI)
                            )
                        );
                    } while (xPathIterC.Current.MoveToNext());
                }
            }

            if (xPathIter.Current != null && xPathIter.Current.MoveToNext())
            {
                XPathNodeIterator xPathIterR = xPathIter.Clone();

                if (xPathIterR.Current != null && xPathIterR.Current.MoveToFirstChild())
                {
                    do
                    {
                        DataRow dR = partialDataTable.NewRow();
                        XPathNodeIterator xPathIterRr = xPathIterR.Clone();

                        if (xPathIterRr.Current != null && xPathIterRr.Current.MoveToFirstChild())
                        {
                            do
                            {
                                string name =
                                    xPathIterRr.Current.GetAttribute("name", xPathIterRr.Current.NamespaceURI);
                                if (name == null) continue;
                                
                                dR[name] = xPathIterRr.Current.Value.ToString(cultureInfo);
                            } while (xPathIterRr.Current.MoveToNext());
                        }

                        partialDataTable.Rows.Add(dR);
                    } while (xPathIterR.Current != null && xPathIterR.Current.MoveToNext());
                }
            }

            if (xPathIter.Current == null || !xPathIter.Current.MoveToNext()) return partialDataTable;
            
            XPathNodeIterator xPathIterE = xPathIter.Clone();

            string messageType =
                xPathIterE.Current?.GetAttribute("messagetype", xPathIterE.Current.NamespaceURI);
            if (string.IsNullOrEmpty(messageType)) messageType = Message.Types.Error.ToString();
            
            partialDataTable.Message =
                new Message(
                    xPathIterE.Current?.Value.ToString(cultureInfo),
                    (Message.Types)System.Enum.Parse(typeof(Message.Types), messageType)
                );

            return partialDataTable;
        }

        private static VariableBlock ParseVariableBlock(ref XPathNodeIterator xPathIter)
        {
            VariableBlock variableBlock =
                new VariableBlock();

            System.Globalization.CultureInfo cultureInfo =
                new System.Globalization.CultureInfo(xPathIter.Current?.GetAttribute("cultureinfo", xPathIter.Current.NamespaceURI));

            if (xPathIter.Current == null || !xPathIter.Current.MoveToFirstChild()) return variableBlock;
            
            XPathNodeIterator xPathIterV = xPathIter.Clone();

            if (xPathIterV.Current == null || !xPathIterV.Current.MoveToFirstChild()) return variableBlock;
            
            do
            {
                variableBlock.Add(
                    xPathIterV.Current.GetAttribute("key", xPathIterV.Current.NamespaceURI),
                    Convert.ChangeType(
                        xPathIterV.Current.Value.ToString(cultureInfo),
                        Service.LoadTypeFromDomain(
                            AppDomain.CurrentDomain,
                            xPathIterV.Current.GetAttribute("type", xPathIterV.Current.NamespaceURI)
                        )
                    )
                );
            } while (xPathIterV.Current.MoveToNext());

            return variableBlock;
        }

        private static bool SearchIsBaseType(Type type, Type searchType)
        {
            do
            {
                if (ReferenceEquals(type, searchType))
                    return true;

                type = type.BaseType;
            } while (type != null && !ReferenceEquals(type, typeof(object)));

            return false;
        }

        private static Type LoadTypeFromDomain(AppDomain appDomain, string searchType)
        {
            Type typeResult = Type.GetType(searchType);

            if (typeResult != null)
                return typeResult;

            Assembly[] assemblies = appDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                typeResult = assembly.GetType(searchType);

                if (typeResult != null)
                    return typeResult;
            }

            return null;
        }

        private static class Serializer
        {
            public static string BinaryToBase64(object @object) =>
                Convert.ToBase64String(Serializer.Serialize(@object));

            public static byte[] Serialize(object @object)
            {
                if (@object == null)
                    return null;

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    {
                        Binder = new OverrideBinder()
                    };

                Stream serializationStream = null;
                try
                {
                    serializationStream = new MemoryStream();

                    binaryFormatter.Serialize(serializationStream, @object);

                    byte[] result = new byte[serializationStream.Position];

                    serializationStream.Seek(0, SeekOrigin.Begin);
                    serializationStream.Read(result, 0, result.Length);

                    return result;
                }
                catch (Exception e)
                {
                    Console.Push(
                        "X Serializer Exception...", 
                        e.Message, 
                        e.ToString(), 
                        false, 
                        true,
                        type: Console.Type.Error);
                    
                    return null;
                }
                finally
                {
                    serializationStream?.Close();
                }
            }

            public static object Base64ToBinary(string serializedString) =>
                Serializer.Deserialize(Convert.FromBase64String(serializedString));

            public static object Deserialize(byte[] serializedBytes)
            {
                if (serializedBytes == null || serializedBytes.Length == 0) 
                    return null;
                
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    {
                        Binder = new OverrideBinder()
                    };

                Stream serializationStream = null;
                try
                {
                    serializationStream = new MemoryStream(serializedBytes);

                    return binaryFormatter.Deserialize(serializationStream);
                }
                catch (Exception e)
                {
                    Console.Push(
                        "X Deserializer Exception...", 
                        e.Message, 
                        e.ToString(), 
                        false, 
                        true,
                        type: Console.Type.Error);
                    
                    return null;
                }
                finally
                {
                    serializationStream?.Close();
                }
            }
        }
    }
}
