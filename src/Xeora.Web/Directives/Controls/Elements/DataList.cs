using System;
using System.Data;
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

        public DirectiveCollection Children => null;
        public bool LinkArguments => false;

        public void Parse()
        {
            this._Children = new DirectiveCollection(this._Parent.Mother, this._Parent);

            this._Parent.Mother.RequestParsing(
                this._SelectedContent == -1 ? this._Contents.MessageTemplate : this._Contents.Parts[this._SelectedContent], ref this._Children, this._Parent.Arguments);
        }

        public void Render(string requesterUniqueID)
        {
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

                    return DirectiveHelper.RenderProperty(this._Parent.Parent, query, this._Parent.Parent.Arguments, requesterUniqueID);
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.ControlResult.IDataSource>(Basics.Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exception.ExecutionException(invokeResult.Exception.Message, invokeResult.Exception.InnerException);

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveID,
                new DataListOutputInfo(this._Parent.UniqueID, 0, 0, true)
            );

            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Deliver(RenderStatus.Rendered, invokeResult.Result.Message.Content);
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                    this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
                }

                return;
            }

            switch (invokeResult.Result.Type)
            {
                case Basics.ControlResult.DataSourceTypes.DirectDataAccess:
                    this.RenderDirectDataAccess(requesterUniqueID, ref invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.ObjectFeed:
                    string vv = string.Empty;
                    foreach (object x in this._Settings.Bind.Parameters.Values)
                        vv += " -- " + x;
                    this.RenderObjectFeed(requesterUniqueID, ref invokeResult, this._Settings.Bind.ToString() + " >> " + vv);

                    break;
                case Basics.ControlResult.DataSourceTypes.PartialDataTable:
                    this.RenderPartialDataTable(requesterUniqueID, ref invokeResult);

                    break;
            }
            // ----
        }

        private void RenderPartialDataTable(string requesterUniqueID, ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            ArgumentCollection dataListArgs =
                new ArgumentCollection();
            bool isItemIndexColumnExists = false;

            DataTable repeaterList =
                (DataTable)invokeResult.Result.GetResult();

            foreach (DataColumn dC in repeaterList.Columns)
            {
                isItemIndexColumnExists =
                    string.Compare(dC.ColumnName, "ItemIndex", StringComparison.InvariantCultureIgnoreCase) == 0;

                dataListArgs.AppendKey(dC.ColumnName);
            }

            StringBuilder renderedContent = new StringBuilder();

            for (int index = 0; index < repeaterList.Rows.Count; index++)
            {
                dataListArgs.Reset(
                    repeaterList.Rows[index].ItemArray);
                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", index);
                // this is for user interaction
                if (!isItemIndexColumnExists)
                    dataListArgs.AppendKeyWithValue("ItemIndex", index);

                this._Parent.Arguments.Replace(dataListArgs);
                this._SelectedContent = index % this._Contents.Parts.Count;

                this.Parse();
                this._Children.Render(requesterUniqueID);

                renderedContent.Append(this._Parent.Result);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveID,
                new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );

            this._Parent.Deliver(RenderStatus.Rendered, renderedContent.ToString());
        }

        private void RenderDirectDataAccess(string requesterUniqueID, ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            IDbCommand dbCommand =
                (IDbCommand)invokeResult.Result.GetResult();

            if (dbCommand == null)
                throw new NullReferenceException(string.Format("DirectDataAccess [{0}] failed! DatabaseCommand must not be null!", this._Parent.DirectiveID));

            IDataReader dbReader = null;
            try
            {
                dbCommand.Connection.Open();
                dbReader = dbCommand.ExecuteReader();

                ArgumentCollection dataListArgs =
                    new ArgumentCollection();
                bool isItemIndexColumnExists = false;

                StringBuilder renderedContent = new StringBuilder();
                int count = 0; long total = -1;

                while(dbReader.Read())
                {
                    dataListArgs.Reset();

                    for (int cC = 0; cC < dbReader.FieldCount; cC++)
                    {
                        if (string.Compare(dbReader.GetName(cC), "_sys_Total", StringComparison.InvariantCultureIgnoreCase) == 0)
                            total = dbReader.GetInt64(cC);

                        isItemIndexColumnExists =
                            string.Compare(dbReader.GetName(cC), "ItemIndex", StringComparison.InvariantCultureIgnoreCase) == 0;

                        dataListArgs.AppendKeyWithValue(dbReader.GetName(cC), dbReader.GetValue(cC));
                    }
                    dataListArgs.AppendKeyWithValue("_sys_ItemIndex", count);
                    // this is for user interaction
                    if (!isItemIndexColumnExists)
                        dataListArgs.AppendKeyWithValue("ItemIndex", count);

                    this._SelectedContent = count % this._Contents.Parts.Count;
                    this._Parent.Arguments.Replace(dataListArgs);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);

                    renderedContent.Append(this._Parent.Result);

                    count++;
                }

                this._Parent.Parent.Arguments.AppendKeyWithValue(
                    this._Parent.DirectiveID,
                    new DataListOutputInfo(this._Parent.UniqueID, count, (total == -1) ? count : total, false)
                );

                this._Parent.Deliver(RenderStatus.Rendered, renderedContent.ToString());
            }
            catch (System.Exception ex)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Deliver(RenderStatus.Rendering, ex.Message);
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", Basics.ControlResult.Message.Types.Error);
                    this._Parent.Arguments.AppendKeyWithValue("Message", ex.Message);

                    this._SelectedContent = -1;
                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                    this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
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

        private void RenderObjectFeed(string requesterUniqueID, ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult, string p)
        {
            if (invokeResult.Result.Message != null)
            {
                if (!this._Contents.HasMessageTemplate)
                    this._Parent.Deliver(RenderStatus.Rendered, invokeResult.Result.Message.Content);
                else
                {
                    this._Parent.Arguments.AppendKeyWithValue("MessageType", invokeResult.Result.Message.Type);
                    this._Parent.Arguments.AppendKeyWithValue("Message", invokeResult.Result.Message.Content);

                    this.Parse();
                    this._Children.Render(requesterUniqueID);
                    this._Parent.Deliver(RenderStatus.Rendered, this._Parent.Result);
                }

                return;
            }

            object[] objectList =
                (object[])invokeResult.Result.GetResult();

            ArgumentCollection dataListArgs =
                new ArgumentCollection();

            StringBuilder renderedContent = new StringBuilder();

            for (int index = 0; index < objectList.Length; index++)
            {
                dataListArgs.Reset();

                dataListArgs.AppendKeyWithValue("CurrentObject", objectList[index]);
                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", index);
                dataListArgs.AppendKeyWithValue("ItemIndex", index);

                this._SelectedContent = index % this._Contents.Parts.Count;
                this._Parent.Arguments.Replace(dataListArgs);

                this.Parse();
                this._Children.Render(requesterUniqueID);

                renderedContent.Append(this._Parent.Result);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveID,
                new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );

            this._Parent.Deliver(RenderStatus.Rendered, renderedContent.ToString());
        }
    }
}