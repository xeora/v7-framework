using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Xeora.Web.Site.Setting
{
    public class xService : Basics.Domain.IxService
    {
        public string CreateAuthentication(params DictionaryEntry[] items)
        {
            if (items != null)
            {
                string publicKey = Guid.NewGuid().ToString();

                Global.xServiceSessionInfo sessionInfo =
                    new Global.xServiceSessionInfo(publicKey, DateTime.Now);

                foreach (DictionaryEntry item in items)
                    sessionInfo.AddSessionItem(item.Key.ToString(), item.Value);

                this.VariablePool.Set(publicKey, sessionInfo);
            }

            return null;
        }

        public object ReadSessionVariable(string publicKey, string name)
        {
            Global.xServiceSessionInfo sessionInfo =
                (Global.xServiceSessionInfo)this.VariablePool.Get(publicKey);

            if (sessionInfo == null)
                return null;

            int TimeoutMinute =
                Basics.Configurations.Xeora.Session.Timeout;
            object rObject = null;

            if (DateTime.Compare(sessionInfo.SessionDate.AddMinutes(TimeoutMinute), DateTime.Now) > 0)
            {
                rObject = sessionInfo[name];
                sessionInfo.SessionDate = DateTime.Now;
            }
            else
                sessionInfo = null;

            this.VariablePool.Set(publicKey, sessionInfo);

            return rObject;
        }

        public string Render(string executeIn, string serviceID)
        {
            // call = Calling Function Providing in Query String
            Basics.Execution.Bind bind =
                Basics.Execution.Bind.Make(
                    string.Format(
                        "{0}?{1}.{2},~xParams",
                        executeIn,
                        serviceID,
                        Basics.Helpers.Context.Request.QueryString["call"]
                    )
                );

            bind.Parameters.Prepare(
                (parameter) =>
                {
                    Basics.X.ServiceParameterCollection serviceParameterCol =
                        new Basics.X.ServiceParameterCollection();

                    try
                    {
                        serviceParameterCol.ParseXML(
                            Basics.Helpers.Context.Request.Body.Form[parameter.Key]);                            
                    }
                    catch (System.Exception)
                    { }

                    return serviceParameterCol;
                }
            );

            Basics.Execution.InvokeResult<object> invokeResult =
                Manager.AssemblyCore.InvokeBind<object>(Basics.Helpers.Context.Request.Header.Method, bind, Manager.ExecuterTypes.Undefined);

            return this.GenerateXML(invokeResult.Result);
        }

        public string GenerateXML(object result)
        {
            StringWriter xmlStream = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(xmlStream);

            // Start Document Element
            bool isDone = !(result != null && result is System.Exception);

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
                if (result is Basics.ControlResult.RedirectOrder)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);

                    xmlWriter.WriteCData(
                        ((Basics.ControlResult.RedirectOrder)result).Location);
                }
                else if (result is Basics.ControlResult.Message)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteAttributeString("messagetype", ((Basics.ControlResult.Message)result).Type.ToString());

                    xmlWriter.WriteCData(
                        ((Basics.ControlResult.Message)result).Content);
                }
                else if (result is Basics.ControlResult.Conditional)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);

                    xmlWriter.WriteCData(
                        ((Basics.ControlResult.Conditional)result).Result.ToString());
                }
                else if (result is Basics.ControlResult.VariableBlock)
                {
                    Basics.ControlResult.VariableBlock variableBlockResult =
                        (Basics.ControlResult.VariableBlock)result;

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

                            xmlWriter.WriteCData(((Basics.ControlResult.VariableBlock)result)[key].ToString());
                        }

                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                }
                else if (result is Basics.ControlResult.DirectDataAccess)
                {
                    System.Exception ex = new System.Exception("DirectDataAccess is not a transferable object!");

                    xmlWriter.WriteAttributeString("type", ex.GetType().FullName);

                    xmlWriter.WriteCData(ex.Message);
                }
                else if (result is Basics.ControlResult.ObjectFeed)
                {
                    // TODO: Object Feed xService Implementation should be done!
                    System.Exception ex = new NotImplementedException();

                    xmlWriter.WriteAttributeString("type", ex.GetType().FullName);

                    xmlWriter.WriteCData(ex.Message);
                }
                else if (result is Basics.ControlResult.PartialDataTable)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().Name);
                    xmlWriter.WriteAttributeString("total", ((Basics.ControlResult.PartialDataTable)result).Total.ToString());
                    xmlWriter.WriteAttributeString("cultureinfo", CultureInfo.CurrentCulture.ToString());

                    DataTable tDT = ((Basics.ControlResult.PartialDataTable)result).Copy();

                    xmlWriter.WriteStartElement("Columns");
                    foreach (DataColumn dC in tDT.Columns)
                    {
                        xmlWriter.WriteStartElement("Column");
                        xmlWriter.WriteAttributeString("name", dC.ColumnName);
                        xmlWriter.WriteAttributeString("type", dC.DataType.FullName);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    int rowIndex = 0;

                    xmlWriter.WriteStartElement("Rows");
                    foreach (DataRow dR in tDT.Rows)
                    {
                        xmlWriter.WriteStartElement("Row");
                        xmlWriter.WriteAttributeString("index", rowIndex.ToString());
                        foreach (DataColumn dC in tDT.Columns)
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

                    if (((Basics.ControlResult.PartialDataTable)result).Message != null)
                    {
                        xmlWriter.WriteStartElement("Message");
                        xmlWriter.WriteAttributeString("messagetype", ((Basics.ControlResult.PartialDataTable)result).Message.Type.ToString());
                        xmlWriter.WriteCData(((Basics.ControlResult.PartialDataTable)result).Message.Content);
                        xmlWriter.WriteEndElement();
                    }
                }
                else if (result is System.Exception)
                {
                    xmlWriter.WriteAttributeString("type", result.GetType().FullName);

                    xmlWriter.WriteCData(((System.Exception)result).Message);
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
                            string SerializedValue = Helper.Serialization.Quick.BinaryToBase64Serialize(result);

                            xmlWriter.WriteAttributeString("type", result.GetType().FullName);

                            xmlWriter.WriteCData(SerializedValue);
                        }
                        catch (System.Exception ex)
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

            return xmlStream.ToString();
        }

        private Basics.Service.VariablePoolOperation VariablePool => Basics.Helpers.VariablePoolForxService;
    }
}