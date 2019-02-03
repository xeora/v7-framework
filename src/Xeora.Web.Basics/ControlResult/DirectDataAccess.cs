using System;
using System.Data;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class DirectDataAccess : IDataSource
    {
        private readonly IDbCommand _DbCommand;

        public DirectDataAccess(IDbCommand dbCommand)
        {
            this.Type = DataSourceTypes.DirectDataAccess;
            this.Message = null;

            if (dbCommand != null)
            {
                if (dbCommand.Connection == null)
                    throw new NullReferenceException("Connection Parameter of Database Command must be available and valid!");

                if (string.IsNullOrEmpty(dbCommand.CommandText))
                    throw new NullReferenceException("CommandText Parameter of Database Command must be available and valid!");
            }

            this._DbCommand = dbCommand;
        }

        public DataSourceTypes Type { get; private set; }
        public Message Message { get; set; }
        public long Count => 0;
        public long Total { get; set; }

        public object GetResult()
        {
            if (this._DbCommand == null)
                return null;

            return this._DbCommand;
        }
    }
}