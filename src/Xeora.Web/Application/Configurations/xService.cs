using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Xeora.Web.Application.Configurations
{
    public class xService : Basics.Domain.IxService
    {
        public string CreateAuthentication(params DictionaryEntry[] items)
        {
            if (items == null) return null;
            
            string publicKey = Guid.NewGuid().ToString();

            Global.xServiceSessionInfo sessionInfo =
                new Global.xServiceSessionInfo(publicKey, DateTime.Now);

            foreach (DictionaryEntry item in items)
                sessionInfo.AddSessionItem(item.Key.ToString(), item.Value);

            xService.VariablePool.Set(publicKey, sessionInfo);

            return null;
        }

        public object ReadSessionVariable(string publicKey, string name)
        {
            Global.xServiceSessionInfo sessionInfo =
                (Global.xServiceSessionInfo)xService.VariablePool.Get(publicKey);

            if (sessionInfo == null)
                return null;

            int timeoutMinute =
                Basics.Configurations.Xeora.Session.Timeout;
            object rObject = null;

            if (DateTime.Compare(sessionInfo.SessionDate.AddMinutes(timeoutMinute), DateTime.Now) > 0)
            {
                rObject = sessionInfo[name];
                sessionInfo.SessionDate = DateTime.Now;
            }
            else
                sessionInfo = null;

            xService.VariablePool.Set(publicKey, sessionInfo);

            return rObject;
        }

        public Basics.RenderResult Render(string executeIn, string serviceId)
        {
            // call = Calling Function Providing in Query String
            Basics.Execution.Bind bind =
                Basics.Execution.Bind.Make(
                    string.Format(
                        "{0}?{1}.{2},~xParams",
                        executeIn,
                        serviceId,
                        Basics.Helpers.Context.Request.QueryString["call"]
                    )
                );

            bind.Parameters.Prepare(
                parameter =>
                {
                    Basics.X.ServiceParameterCollection serviceParameterCol =
                        new Basics.X.ServiceParameterCollection();

                    try
                    {
                        serviceParameterCol.ParseXml(
                            Basics.Helpers.Context.Request.Body.Form[parameter.Key]);                            
                    }
                    catch
                    { /* Just handle Exceptions */ }

                    return serviceParameterCol;
                }
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.Executer.InvokeBind<object>(Basics.Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Undefined);

            return this.GenerateXml(invokeResult.Result);
        }

        public Basics.RenderResult GenerateXml(object result)
        {
            StringWriter xmlStream = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(xmlStream);

            // Start Document Element
            bool isDone = !(result is Exception);

            xmlWriter.WriteStartElement("ServiceResult");
            xmlWriter.WriteAttributeString("isdone", isDone.ToString());

            xmlWriter.WriteStartElement("Item");

            if (result == null)
            {
                xmlWriter.WriteAttributeString("type", typeof(string).FullName);
                xmlWriter.WriteCData(string.Empty);
            }
            else
            {
                if (result is Basics.ControlResult.RedirectOrder redirectOrder)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteCData(redirectOrder.Location);
                }
                else if (result is Basics.ControlResult.Message message)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteAttributeString("messagetype", message.Type.ToString());
                    xmlWriter.WriteCData(message.Content);
                }
                else if (result is Basics.ControlResult.Conditional conditional)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteCData(conditional.Result.ToString());
                }
                else if (result is Basics.ControlResult.VariableBlock variableBlockResult)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteAttributeString("cultureinfo", CultureInfo.CurrentCulture.ToString());

                    xmlWriter.WriteStartElement("VariableList");
                    foreach (string key in variableBlockResult.Keys)
                    {
                        xmlWriter.WriteStartElement("Variable");
                        xmlWriter.WriteAttributeString("key", key);

                        if (result.GetType().IsPrimitive)
                        {
                            xmlWriter.WriteAttributeString("type", result.GetType().FullName);
                            xmlWriter.WriteCData(Convert.ToString(result));
                        }
                        else
                        {
                            xmlWriter.WriteAttributeString("type", typeof(string).FullName);
                            xmlWriter.WriteCData(result.ToString());
                        }

                        if (variableBlockResult[key] == null)
                        {
                            xmlWriter.WriteAttributeString("type", typeof(object).FullName);
                            xmlWriter.WriteCData(string.Empty);
                        }
                        else
                        {
                            if (variableBlockResult[key].GetType().IsPrimitive)
                                xmlWriter.WriteAttributeString("type", variableBlockResult[key].GetType().FullName);
                            else
                                xmlWriter.WriteAttributeString("type", typeof(string).FullName);
                            xmlWriter.WriteCData(variableBlockResult[key].ToString());
                        }

                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                }
                else if (result is Basics.ControlResult.DirectDataAccess)
                {
                    Exception ex = new Exception("DirectDataAccess is not a transferable object!");

                    xmlWriter.WriteAttributeString("type", ex.GetType().FullName);
                    xmlWriter.WriteCData(ex.Message);
                }
                else if (result is Basics.ControlResult.ObjectFeed)
                {
                    // TODO: Object Feed xService Implementation should be done!
                    Exception ex = new NotImplementedException();

                    xmlWriter.WriteAttributeString("type", ex.GetType().FullName);
                    xmlWriter.WriteCData(ex.Message);
                }
                else if (result is Basics.ControlResult.PartialDataTable partialDataTable)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteAttributeString("total", partialDataTable.Total.ToString());
                    xmlWriter.WriteAttributeString("cultureinfo", CultureInfo.CurrentCulture.ToString());

                    DataTable dT = partialDataTable.Copy();

                    xmlWriter.WriteStartElement("Columns");
                    foreach (DataColumn dC in dT.Columns)
                    {
                        xmlWriter.WriteStartElement("Column");
                        xmlWriter.WriteAttributeString("name", dC.ColumnName);
                        xmlWriter.WriteAttributeString("type", dC.DataType.FullName);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    int rowIndex = 0;

                    xmlWriter.WriteStartElement("Rows");
                    foreach (DataRow dR in dT.Rows)
                    {
                        xmlWriter.WriteStartElement("Row");
                        xmlWriter.WriteAttributeString("index", rowIndex.ToString());
                        foreach (DataColumn dC in dT.Columns)
                        {
                            xmlWriter.WriteStartElement("Item");
                            xmlWriter.WriteAttributeString("name", dC.ColumnName);
                            xmlWriter.WriteCData(dR[dC.ColumnName].ToString());
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();

                        rowIndex += 1;
                    }
                    xmlWriter.WriteEndElement();

                    if (partialDataTable.Message != null)
                    {
                        xmlWriter.WriteStartElement("Message");
                        xmlWriter.WriteAttributeString("messagetype", partialDataTable.Message.Type.ToString());
                        xmlWriter.WriteCData(partialDataTable.Message.Content);
                        xmlWriter.WriteEndElement();
                    }
                }
                else if (result is Exception exception)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().FullName);
                    xmlWriter.WriteCData(exception.Message);
                }
                else
                {
                    if (result.GetType().IsPrimitive)
                    {
                        xmlWriter.WriteAttributeString("type", result.GetType().FullName);
                        xmlWriter.WriteCData(Convert.ToString(result));
                    }
                    else
                    {
                        try
                        {
                            string serializedValue = 
                                Tools.Serialization.Quick.BinaryToBase64Serialize(result);

                            xmlWriter.WriteAttributeString("type", result.GetType().FullName);
                            xmlWriter.WriteCData(serializedValue);
                        }
                        catch (Exception ex)
                        {
                            xmlWriter.WriteAttributeString("type", ex.GetType().FullName);
                            xmlWriter.WriteCData(ex.Message);
                        }
                    }

                }
            }

            xmlWriter.WriteEndElement();

            // End Document Element
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
            xmlWriter.Close();
            xmlStream.Close();

            return new Basics.RenderResult(xmlStream.ToString(), false);
        }

        private static Basics.Service.VariablePoolOperation VariablePool => Basics.Helpers.VariablePoolForxService;
    }
}