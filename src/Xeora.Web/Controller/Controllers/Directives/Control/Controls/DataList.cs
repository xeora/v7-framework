using System;
using System.Data;
using System.Globalization;
using System.Text;
using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Controller.Directive.Control
{
    public class DataList : ControlWithContent, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;

        public DataList(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments, ControlSettings settings) :
            base(rawStartIndex, rawValue, contentArguments, settings)
        { }

        public override IControl Clone() =>
            new DataList(this.RawStartIndex, this.RawValue, this.ContentArguments, this.Settings);
        
        protected override void RenderControl(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            // Reset Variables
            Basics.Helpers.VariablePool.Set(this.ControlID, null);

            // Call Related Function and Exam It
            IController leveledController = this;
            int level = this.Leveling.Level;

            do
            {
                if (level == 0)
                    break;

                leveledController = leveledController.Parent;

                if (leveledController is Renderless)
                    leveledController = leveledController.Parent;

                level -= 1;
            } while (leveledController != null);

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this.Bind.Parameters.Prepare(
                (parameter) =>
                {
                    Property property = new Property(0, parameter.Query, (leveledController.Parent == null ? null : leveledController.Parent.ContentArguments));
                    property.Mother = leveledController.Mother;
                    property.Parent = leveledController.Parent;
                    property.InstanceRequested += (ref IDomain instance) => InstanceRequested?.Invoke(ref instance);
                    property.Setup();

                    property.Render(requesterUniqueID);

                    return property.ObjectResult;
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.IDataSource>(Basics.Helpers.Context.Request.Header.Method, this.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            Basics.Helpers.VariablePool.Set(this.ControlID, new Global.DataListOutputInfo(this.UniqueID, 0, 0, true));

            switch (invokeResult.Result.Type)
            {
                case Basics.ControlResult.DataSourceTypes.DirectDataAccess:
                    this.RenderDirectDataAccess(requesterUniqueID, invokeResult, contentDescription);

                    break;
                case Basics.ControlResult.DataSourceTypes.ObjectFeed:
                    this.RenderObjectFeed(requesterUniqueID, invokeResult, contentDescription);

                    break;
                case Basics.ControlResult.DataSourceTypes.PartialDataTable:
                    this.RenderPartialDataTable(requesterUniqueID, invokeResult, contentDescription);

                    break;
            }
            // ----

            this.Mother.Scheduler.Fire(this.ControlID);
        }

        public override void Build()
        {
            // Just override to bypass the base builder.
        }

        private void RenderPartialDataTable(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult, Global.ContentDescription contentDescription)
        {
            DataTable repeaterList =
                (DataTable)invokeResult.Result.GetResult();

            Global.ArgumentInfoCollection dataListArgs =
                new Global.ArgumentInfoCollection();

            if (invokeResult.Result.Message != null)
            {
                if (!contentDescription.HasMessageTemplate)
                    this.RenderedValue = invokeResult.Result.Message.Content;
                else
                {
                    dataListArgs.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    dataListArgs.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.RenderedValue =
                        ControllerHelper.RenderSingleContent(
                            contentDescription.MessageTemplate, this, dataListArgs, requesterUniqueID);
                }

                return;
            }

            Basics.Helpers.VariablePool.Set(this.ControlID, new Global.DataListOutputInfo(this.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false));

            CultureInfo compareCulture = new CultureInfo("en-US");

            StringBuilder renderedContent = new StringBuilder();
            int contentIndex = 0, rC = 0;
            bool isItemIndexColumnExists = false;

            foreach (DataColumn dC in repeaterList.Columns)
            {
                if (compareCulture.CompareInfo.Compare(dC.ColumnName, "ItemIndex", CompareOptions.IgnoreCase) == 0)
                    isItemIndexColumnExists = true;

                dataListArgs.AppendKey(dC.ColumnName);
            }
            dataListArgs.AppendKey("_sys_ItemIndex");
            repeaterList.Columns.Add("_sys_ItemIndex", typeof(int));
            // this is for user interaction
            if (!isItemIndexColumnExists)
            {
                dataListArgs.AppendKey("ItemIndex");
                repeaterList.Columns.Add("ItemIndex", typeof(int));
            }

            foreach (DataRow dR in repeaterList.Rows)
            {
                object[] dRValues = dR.ItemArray;

                if (!isItemIndexColumnExists)
                {
                    dRValues[dRValues.Length - 2] = rC;
                    dRValues[dRValues.Length - 1] = rC;
                }
                else
                    dRValues[dRValues.Length - 1] = rC;

                dataListArgs.Reset(dRValues);

                contentIndex = rC % contentDescription.Parts.Count;

                string renderedRow =
                    ControllerHelper.RenderSingleContent(
                        contentDescription.Parts[contentIndex], this, dataListArgs, requesterUniqueID);

                renderedContent.Append(renderedRow);

                rC += 1;
            }

            this.RenderedValue = renderedContent.ToString();
        }

        private void RenderDirectDataAccess(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult, Global.ContentDescription contentDescription)
        {
            IDbCommand dbCommand =
                (IDbCommand)invokeResult.Result.GetResult();

            Global.ArgumentInfoCollection dataListArgs =
                new Global.ArgumentInfoCollection();

            if (dbCommand == null)
            {
                if (invokeResult.Result.Message != null)
                {
                    if (!contentDescription.HasMessageTemplate)
                        this.RenderedValue = invokeResult.Result.Message.Content;
                    else
                    {
                        dataListArgs.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                        dataListArgs.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                        this.RenderedValue =
                            ControllerHelper.RenderSingleContent(
                                contentDescription.MessageTemplate, this, dataListArgs, requesterUniqueID);
                    }

                    Helper.EventLogger.Log(string.Format("DirectDataAccess [{0}] failed! DatabaseCommand must not be null!", this.ControlID));
                }
                else
                    throw new NullReferenceException(string.Format("DirectDataAccess [{0}] failed! DatabaseCommand must not be null!", this.ControlID));

                return;
            }

            IDataReader dbReader = null;
            try
            {
                dbCommand.Connection.Open();
                dbReader = dbCommand.ExecuteReader();

                CultureInfo compareCulture = new CultureInfo("en-US");

                StringBuilder renderedContent = new StringBuilder();
                int contentIndex = 0, rC = 0;
                bool isItemIndexColumnExists = false;

                if (!dbReader.Read())
                {
                    if (invokeResult.Result.Message != null)
                    {
                        if (!contentDescription.HasMessageTemplate)
                            this.RenderedValue = invokeResult.Result.Message.Content;
                        else
                        {
                            dataListArgs.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                            dataListArgs.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                            this.RenderedValue =
                                ControllerHelper.RenderSingleContent(
                                    contentDescription.MessageTemplate, this, dataListArgs, requesterUniqueID);
                        }
                    }

                    return;
                }

                do
                {
                    dataListArgs.Reset();

                    for (int cC = 0; cC < dbReader.FieldCount; cC++)
                    {
                        if (compareCulture.CompareInfo.Compare(dbReader.GetName(cC), "ItemIndex", CompareOptions.IgnoreCase) == 0)
                            isItemIndexColumnExists = true;

                        dataListArgs.AppendKeyWithValue(dbReader.GetName(cC), dbReader.GetValue(cC));
                    }
                    dataListArgs.AppendKeyWithValue("_sys_ItemIndex", rC);
                    // this is for user interaction
                    if (!isItemIndexColumnExists)
                        dataListArgs.AppendKeyWithValue("ItemIndex", rC);

                    contentIndex = rC % contentDescription.Parts.Count;

                    string renderedRow =
                        ControllerHelper.RenderSingleContent(
                            contentDescription.Parts[contentIndex], this, dataListArgs, requesterUniqueID);

                    renderedContent.Append(renderedRow);

                    rC += 1;
                } while (dbReader.Read());

                Basics.Helpers.VariablePool.Set(this.ControlID, new Global.DataListOutputInfo(this.UniqueID, rC, rC, false));
                this.RenderedValue = renderedContent.ToString();
            }
            catch (System.Exception ex)
            {
                if (invokeResult.Result.Message == null)
                    throw new Exception.DirectDataAccessException(ex);

                if (!contentDescription.HasMessageTemplate)
                    this.RenderedValue = invokeResult.Result.Message.Content;
                else
                {
                    dataListArgs.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    dataListArgs.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.RenderedValue =
                        ControllerHelper.RenderSingleContent(
                            contentDescription.MessageTemplate, this, dataListArgs, requesterUniqueID);
                }

                Helper.EventLogger.Log(ex);
            }
            finally
            {
                if (dbReader != null)
                {
                    dbReader.Close();
                    dbReader.Dispose();
                    GC.SuppressFinalize(dbReader);
                }

                if (dbCommand != null)
                {
                    if (dbCommand.Connection.State == ConnectionState.Open)
                        dbCommand.Connection.Close();

                    dbCommand.Dispose();
                    GC.SuppressFinalize(dbCommand);
                }
            }
        }

        private void RenderObjectFeed(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult, Global.ContentDescription contentDescription)
        {
            object[] objectList =
                (object[])invokeResult.Result.GetResult();

            Global.ArgumentInfoCollection dataListArgs =
                new Global.ArgumentInfoCollection();

            if (invokeResult.Result.Message != null)
            {
                if (!contentDescription.HasMessageTemplate)
                    this.RenderedValue = invokeResult.Result.Message.Content;
                else
                {
                    dataListArgs.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    dataListArgs.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.RenderedValue =
                        ControllerHelper.RenderSingleContent(
                            contentDescription.MessageTemplate, this, dataListArgs, requesterUniqueID);
                }

                return;
            }

            Basics.Helpers.VariablePool.Set(this.ControlID, new Global.DataListOutputInfo(this.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false));

            StringBuilder renderedContent = new StringBuilder();
            int contentIndex = 0, rC = 0;

            foreach (object current in objectList)
            {
                dataListArgs.Reset();

                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", rC);
                dataListArgs.AppendKeyWithValue("ItemIndex", rC);

                dataListArgs.AppendKeyWithValue("CurrentObject", current);

                contentIndex = rC % contentDescription.Parts.Count;

                string renderedRow =
                    ControllerHelper.RenderSingleContent(
                        contentDescription.Parts[contentIndex], this, dataListArgs, requesterUniqueID);

                renderedContent.Append(renderedRow);

                rC += 1;
            }

            this.RenderedValue = renderedContent.ToString();
        }
    }
}