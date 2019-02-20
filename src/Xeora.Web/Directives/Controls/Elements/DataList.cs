using System;
using System.Data;
using System.Globalization;
using System.Text;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class DataList : IControl
    {
        private int _SelectedContent = -1;

        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Site.Setting.Control.DataList _Settings;
        private DirectiveCollection _Children;

        public DataList(Control parent, ContentDescription contents, string[] parameters, Site.Setting.Control.DataList settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public bool Searchable => false;

        public void Parse()
        {
            this._Children = new DirectiveCollection(this._Parent.Mother, this._Parent);

            this._Parent.Mother.RequestParsing(
                this._SelectedContent == -1 ? this._Contents.MessageTemplate : this._Contents.Parts[this._SelectedContent], ref this._Children, this._Parent.Arguments);
        }

        public void Render(string requesterUniqueID)
        {
            // Reset Variables
            Basics.Helpers.VariablePool.Set(this._Parent.DirectiveID, null);

            // Call Related Function and Exam It
            IDirective leveledDirective = this._Parent;
            int level = this._Parent.Leveling.Level;

            do
            {
                if (level == 0)
                    break;

                leveledDirective = leveledDirective.Parent;

                if (leveledDirective is Renderless)
                    leveledDirective = leveledDirective.Parent;

                level -= 1;
            } while (leveledDirective != null);

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this._Settings.Bind.Parameters.Prepare(
                (parameter) =>
                {
                    string query = parameter.Query;

                    int paramIndex =
                        DirectiveHelper.CaptureParameterPointer(query);

                    if (paramIndex > -1)
                    {
                        if (paramIndex >= this._Parameters.Length)
                            throw new Exception.FormatIndexOutOfRangeException();

                        query = this._Parameters[paramIndex];
                    }

                    return DirectiveHelper.RenderProperty(leveledDirective.Parent, query, leveledDirective.Parent.Arguments, requesterUniqueID);
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.IDataSource>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            Basics.Helpers.VariablePool.Set(this._Parent.DirectiveID, new DataListOutputInfo(this._Parent.UniqueID, 0, 0, true));

            switch (invokeResult.Result.Type)
            {
                case Basics.ControlResult.DataSourceTypes.DirectDataAccess:
                    this.RenderDirectDataAccess(requesterUniqueID, invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.ObjectFeed:
                    this.RenderObjectFeed(requesterUniqueID, invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.PartialDataTable:
                    this.RenderPartialDataTable(requesterUniqueID, invokeResult);

                    break;
            }
            // ----
        }

        private void RenderPartialDataTable(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            DataTable repeaterList =
                (DataTable)invokeResult.Result.GetResult();

            ArgumentCollection dataListArgs =
                new ArgumentCollection();

            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Result = invokeResult.Result.Message.Content;
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                }

                return;
            }

            Basics.Helpers.VariablePool.Set(this._Parent.DirectiveID, new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false));

            CultureInfo compareCulture = new CultureInfo("en-US");

            StringBuilder renderedContent = new StringBuilder();
            int rC = 0;
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

                this._Parent.Arguments.Replace(dataListArgs);
                this._SelectedContent = rC % this._Contents.Parts.Count;

                this.Parse();
                this._Children.Render(requesterUniqueID);

                renderedContent.Append(this._Parent.Result);

                rC += 1;
            }

            this._Parent.Result = renderedContent.ToString();
        }

        private void RenderDirectDataAccess(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            IDbCommand dbCommand =
                (IDbCommand)invokeResult.Result.GetResult();

            ArgumentCollection dataListArgs =
                new ArgumentCollection();

            if (dbCommand == null)
            {
                if (invokeResult.Result.Message != null)
                {
                    if (!this._Contents.HasMessageTemplate)
                        this._Parent.Result = invokeResult.Result.Message.Content;
                    else
                    {
                        this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                        this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                        this.Parse();
                        this._Children.Render(requesterUniqueID);
                    }

                    Helper.EventLogger.Log(string.Format("DirectDataAccess [{0}] failed! DatabaseCommand must not be null!", this._Parent.DirectiveID));
                }
                else
                    throw new NullReferenceException(string.Format("DirectDataAccess [{0}] failed! DatabaseCommand must not be null!", this._Parent.DirectiveID));

                return;
            }

            IDataReader dbReader = null;
            try
            {
                dbCommand.Connection.Open();
                dbReader = dbCommand.ExecuteReader();

                CultureInfo compareCulture = new CultureInfo("en-US");

                StringBuilder renderedContent = new StringBuilder();
                int rC = 0;
                bool isItemIndexColumnExists = false;

                if (!dbReader.Read())
                {
                    if (invokeResult.Result.Message != null)
                    {
                        if (!this._Contents.HasMessageTemplate)
                            this._Parent.Result = invokeResult.Result.Message.Content;
                        else
                        {
                            this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                            this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                            this.Parse();
                            this._Children.Render(requesterUniqueID);
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

                    this._SelectedContent = rC % this._Contents.Parts.Count;
                    this._Parent.Arguments.Replace(dataListArgs);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);

                    renderedContent.Append(this._Parent.Result);

                    rC += 1;
                } while (dbReader.Read());

                Basics.Helpers.VariablePool.Set(this._Parent.DirectiveID, new DataListOutputInfo(this._Parent.UniqueID, rC, rC, false));
                this._Parent.Result = renderedContent.ToString();
            }
            catch (System.Exception ex)
            {
                if (invokeResult.Result.Message == null)
                    throw new Exception.DirectDataAccessException(ex);

                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Result = invokeResult.Result.Message.Content;
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this._SelectedContent = -1;
                    this.Parse();
                    this._Children.Render(requesterUniqueID);
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

        private void RenderObjectFeed(string requesterUniqueID, Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            object[] objectList =
                (object[])invokeResult.Result.GetResult();

            ArgumentCollection dataListArgs =
                new ArgumentCollection();

            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Result = invokeResult.Result.Message.Content;
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                }

                return;
            }

            Basics.Helpers.VariablePool.Set(this._Parent.DirectiveID, new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false));

            StringBuilder renderedContent = new StringBuilder();
            int rC = 0;

            foreach (object current in objectList)
            {
                dataListArgs.Reset();

                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", rC);
                dataListArgs.AppendKeyWithValue("ItemIndex", rC);

                dataListArgs.AppendKeyWithValue("CurrentObject", current);

                this._SelectedContent = rC % this._Contents.Parts.Count;
                this._Parent.Arguments.Replace(dataListArgs);

                this.Parse();
                this._Children.Render(requesterUniqueID);

                renderedContent.Append(this._Parent.Result);

                rC += 1;
            }

            this._Parent.Result = renderedContent.ToString();
        }
    }
}