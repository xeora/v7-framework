using System;
using System.Data;
using Xeora.Web.Basics;
using Xeora.Web.Directives.Elements;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class DataList : IControl
    {
        private readonly Control _Parent;
        private readonly ContentDescription _Contents;
        private readonly string[] _Parameters;
        private readonly Application.Controls.DataList _Settings;

        public DataList(Control parent, ContentDescription contents, string[] parameters, Application.Controls.DataList settings)
        {
            this._Parent = parent;
            this._Contents = contents;
            this._Parameters = parameters;
            this._Settings = settings;
        }

        public bool LinkArguments => true;
        
        public void Parse()
        {
            if (this._Settings.Bind == null)
                throw new ArgumentNullException(nameof(this._Settings.Bind));

            // Execution preparation should be done at the same level with it's parent. Because of that, send parent as parameters
            this._Settings.Bind.Parameters.Prepare(
                parameter =>
                {
                    string query = parameter.Query;
                    int paramIndex =
                        DirectiveHelper.CaptureParameterPointer(query);

                    if (paramIndex < 0)
                        return Property.Render(this._Parent, query).Item2;
                    
                    if (paramIndex >= this._Parameters.Length)
                        throw new Exceptions.FormatIndexOutOfRangeException("DataList");

                    return Property.Render(this._Parent, this._Parameters[paramIndex]).Item2;
                }
            );

            Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult =
                Manager.Executer.InvokeBind<Basics.ControlResult.IDataSource>(Helpers.Context.Request.Header.Method, this._Settings.Bind, Manager.ExecuterTypes.Control);

            if (invokeResult.Exception != null)
                throw new Exceptions.ExecutionException(invokeResult.Exception);

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveId,
                new DataListOutputInfo(this._Parent.UniqueId, 0, 0, true)
            );

            if (invokeResult.Result.Message != null)
            {
                this.RenderError(invokeResult.Result.Message.Type, invokeResult.Result.Message.Content);
                return;
            }

            switch (invokeResult.Result.Type)
            {
                case Basics.ControlResult.DataSourceTypes.DirectDataAccess:
                    this.RenderDirectDataAccess(ref invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.ObjectFeed:
                    this.RenderObjectFeed(ref invokeResult);

                    break;
                case Basics.ControlResult.DataSourceTypes.PartialDataTable:
                    this.RenderPartialDataTable(ref invokeResult);

                    break;
            }
        }

        private void RenderError(Basics.ControlResult.Message.Types errorType, string errorContent)
        {
            if (!this._Contents.HasMessageTemplate)
            {
                this._Parent.Children.Add(new Static(errorContent));
                return;
            }

            this._Parent.Arguments.AppendKeyWithValue("MessageType", errorType);
            this._Parent.Arguments.AppendKeyWithValue("Message", errorContent);

            this.RenderRow(-1, this._Parent.Arguments);
        }

        private void RenderRow(int index, ArgumentCollection arguments)
        {
            SingleAsync rowSingle =
                new SingleAsync(
                    index == -1
                        ? this._Contents.MessageTemplate
                        : this._Contents.Parts[index % this._Contents.Parts.Count], 
                    arguments.Clone()
                );
            
            this._Parent.Children.Add(rowSingle);
        }

        private void RenderPartialDataTable(ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
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

                this.RenderRow(index, dataListArgs);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveId,
                new DataListOutputInfo(this._Parent.UniqueId, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );
        }

        private void RenderDirectDataAccess(ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
        {
            IDbCommand dbCommand =
                (IDbCommand)invokeResult.Result.GetResult();

            if (dbCommand == null)
                throw new NullReferenceException(
                    $"DirectDataAccess [{this._Parent.DirectiveId}] failed! DatabaseCommand must not be null!");

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

                    this.RenderRow(count, dataListArgs);

                    count++;
                }

                this._Parent.Parent.Arguments.AppendKeyWithValue(
                    this._Parent.DirectiveId,
                    new DataListOutputInfo(this._Parent.UniqueId, count, total == -1 ? count : total, false)
                );
            }
            catch (Exception ex)
            {
                this.RenderError(Basics.ControlResult.Message.Types.Error, ex.Message);

                Basics.Console.Push("Execution Exception...", ex.Message, ex.StackTrace, false, true, type: Basics.Console.Type.Error);
            }
            finally
            {
                if (dbReader != null)
                {
                    dbReader.Close();
                    dbReader.Dispose();
                }

                if (dbCommand.Connection.State == ConnectionState.Open)
                    dbCommand.Connection.Close();

                dbCommand.Dispose();
            }
        }

        private void RenderObjectFeed(ref Basics.Execution.InvokeResult<Basics.ControlResult.IDataSource> invokeResult)
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

                this.RenderRow(index, dataListArgs);
            }

            this._Parent.Parent.Arguments.AppendKeyWithValue(
                this._Parent.DirectiveId,
                new DataListOutputInfo(this._Parent.UniqueId, invokeResult.Result.Count, invokeResult.Result.Total, false)
            );
        }
    }
}