using System;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class ObjectFeed : IDataSource
    {
        private readonly object[] _Objects;
        private long _Total;

        public ObjectFeed(object[] objects)
        {
            this.Type = DataSourceTypes.ObjectFeed;
            this.Message = null;

            if (objects == null)
                objects = new object[] { };
            this._Objects = objects;
        }

        public DataSourceTypes Type { get; private set; }
        public Message Message { get; set; }
        public long Count => this._Objects.Length;

        public long Total
        {
            get
            {
                if (this._Total == 0)
                    this._Total = this._Objects.Length;

                return this._Total;
            }
            set => this._Total = value;
        }

        public object GetResult() =>
            this._Objects;
    }
}
