using System;
using System.Data;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class PartialDataTable : DataTable, IDataSource
    {
        private long _Total;

        public PartialDataTable() :
            this(new DataTable()) 
        { }

        public PartialDataTable(DataTable source) :
            this(source, Guid.Empty)
        { }
        
        public PartialDataTable(DataTable source, Guid resultId)
        {
            this.Type = DataSourceTypes.PartialDataTable;
            this.Message = null;

            this.ResultId = resultId;
            this.Replace(source);
        }

        public DataSourceTypes Type { get; }
        public Message Message { get; set; }
        public long Count => this.Rows.Count;

        public long Total
        {
            get
            {
                if (this._Total == 0)
                    this._Total = this.Rows.Count;

                return this._Total;
            }
            set => this._Total = value; 
        }

        public Guid ResultId { get; set; }
        public object GetResult() => this;

        public void Replace(DataTable source)
        {
            this.Clear();

            if (source != null)
                this.Merge(source, false, MissingSchemaAction.Add);
        }
    }
}
