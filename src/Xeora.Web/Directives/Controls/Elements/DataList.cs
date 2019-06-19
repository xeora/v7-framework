using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class DataList : IControl
    {
        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Site.Setting.Control.DataList _Settings;

        private readonly ConcurrentQueue<Single> _RowQueue;
        private readonly List<Task> _RowRenderTasks;
        private readonly object _RenderedContentLock;
        private readonly StringBuilder _RenderedContent;

        public DataList(Control parent, ContentDescription contents, string[] parameters, Site.Setting.Control.DataList settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;

            this._RowQueue = new ConcurrentQueue<Single>();
            this._RowRenderTasks = new List<Task>();
            this._RenderedContentLock = new object();
            this._RenderedContent = new StringBuilder();
        }

        public DirectiveCollection Children => null;
        public bool LinkArguments => false;

        public void Parse()
        { }

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
                this.RenderError(requesterUniqueID, invokeResult.Result.Message.Type, invokeResult.Result.Message.Content);

                return;
            }

            switch (invokeResult.Result.Type)
            {
                case Basics.ControlResult.DataSourceTypes.DirectDataAccess:
                    this.RenderDirectDataAccess(requesterUniqueID, ref invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.ObjectFeed:
                    this.RenderObjectFeed(requesterUniqueID, ref invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.PartialDataTable:
                    this.RenderPartialDataTable(requesterUniqueID, ref invokeResult);

                    break;
            }
        }

        private void RenderError(string requesterUniqueID, Basics.ControlResult.Message.Types errorType, string errorContent)
        {
            if (!this._Contents.HasMessageTemplate)
                this._Parent.Deliver(RenderStatus.Rendered, errorContent);
            else
            {
                this._Parent.Arguments.AppendKeyWithValue("MessageType", errorType);
                this._Parent.Arguments.AppendKeyWithValue("Message", errorContent);

                this.RenderRow(requesterUniqueID, -1, this._Parent.Arguments);

                this._Parent.Deliver(RenderStatus.Rendered, this.Result);
            }
        }

        private void RenderRow(string requesterUniqueID, int index, ArgumentCollection arguments)
        {
            string currentHandlerID = Basics.Helpers.CurrentHandlerID;

            Single rowSingle =
                new Single(index == -1 ? this._Contents.MessageTemplate : this._Contents.Parts[index % this._Contents.Parts.Count], arguments.Clone())
                {
                    Mother = this._Parent.Mother,
                    Parent = this._Parent
                };

            if (index == -1)
            {
                while (this._RowQueue.TryDequeue(out Single single)) { }
                this._RowRenderTasks.Clear();
                this._RenderedContent.Clear();
            }

            this._RowQueue.Enqueue(rowSingle);
            this._RowRenderTasks.Add(
                Task.Factory.StartNew(
                    (s) =>
                    {
                        object[] list = (object[])s;

                        string handlerID = (string)list[0];
                        Single single = (Single)list[1];

                        Basics.Helpers.AssignHandlerID(handlerID);
                        single.Render(requesterUniqueID);

                        lock (this._RenderedContentLock)
                        {
                            do
                            {
                                if (!this._RowQueue.TryPeek(out Single queueSingle))
                                    return;

                                if (queueSingle.Status != RenderStatus.Rendered)
                                    return;

                                if (!this._RowQueue.TryDequeue(out queueSingle))
                                    return;

                                this._RenderedContent.Append(queueSingle.Result);
                            } while (true);
                        }
                    },
                    new object[] { currentHandlerID, rowSingle }
                )
            );
        }

        private string Result {
            get
            {
                Task.WaitAll(this._RowRenderTasks.ToArray());

                return this._RenderedContent.ToString();
            }
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

            for (int index = 0; index < repeaterList.Rows.Count; index++)
            {
                dataListArgs.Reset(
                    repeaterList.Rows[index].ItemArray);
                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", index);
                // this is for user interaction
                if (!isItemIndexColumnExists)
                    dataListArgs.AppendKeyWithValue("ItemIndex", index);

                this.RenderRow(requesterUniqueID, index, dataListArgs);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveID,
                new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );

            this._Parent.Deliver(RenderStatus.Rendered, this.Result);
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

                    this.RenderRow(requesterUniqueID, count, dataListArgs);

                    count++;
                }

                this._Parent.Parent.Arguments.AppendKeyWithValue(
                    this._Parent.DirectiveID,
                    new DataListOutputInfo(this._Parent.UniqueID, count, (total == -1) ? count : total, false)
                );

                this._Parent.Deliver(RenderStatus.Rendered, this.Result);
            }
            catch (System.Exception ex)
            {
                this.RenderError(requesterUniqueID, Basics.ControlResult.Message.Types.Error, ex.Message);

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

        private void RenderObjectFeed(string requesterUniqueID, ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            object[] objectList =
                (object[])invokeResult.Result.GetResult();

            ArgumentCollection dataListArgs =
                new ArgumentCollection();

            for (int index = 0; index < objectList.Length; index++)
            {
                dataListArgs.Reset();

                dataListArgs.AppendKeyWithValue("CurrentObject", objectList[index]);
                dataListArgs.AppendKeyWithValue("_sys_ItemIndex", index);
                dataListArgs.AppendKeyWithValue("ItemIndex", index);

                this.RenderRow(requesterUniqueID, index, dataListArgs);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveID,
                new DataListOutputInfo(this._Parent.UniqueID, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );

            this._Parent.Deliver(RenderStatus.Rendered, this.Result);
        }
    }
}