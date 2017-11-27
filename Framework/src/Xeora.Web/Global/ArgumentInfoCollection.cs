using System;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ArgumentInfoCollection
    {
        private Dictionary<string, int> _ArgumentInfoIndexes;
        private object[] _ValueList;

        public ArgumentInfoCollection()
        {
            this._ArgumentInfoIndexes = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this._ValueList = new object[] { };
        }

        public void Reset()
        {
            this._ArgumentInfoIndexes.Clear();
            this._ValueList = new object[] { };
        }

        public void Reset(string[] keys)
        {
            this._ArgumentInfoIndexes.Clear();
            this._ValueList = new object[] { };

            if (keys != null)
            {
                foreach (string key in keys)
                    this.AppendKey(key);
            }
        }

        public void Reset(object[] values)
        {
            this._ValueList = new object[this._ValueList.Length];

            if (values != null)
            {
                if (values.Length != this._ValueList.Length)
                    throw new ArgumentOutOfRangeException(SystemMessages.ARGUMENT_KEYVALUELENGTHMATCH);

                this._ValueList = values;
            }
        }

        public void Replace(ArgumentInfoCollection aIC)
        {
            if (aIC != null)
            {
                this._ArgumentInfoIndexes = aIC._ArgumentInfoIndexes;
                this._ValueList = aIC._ValueList;
            }
        }

        public void AppendKey(string key) =>
            this.AppendKeyWithValue(key, null);

        public void AppendKeyWithValue(string key, object value)
        {
            if (!string.IsNullOrEmpty(key) && 
                !this._ArgumentInfoIndexes.ContainsKey(key))
            {
                // Add Key
                this._ArgumentInfoIndexes.Add(key, this._ArgumentInfoIndexes.Count);

                // Add Value
                Array.Resize<object>(ref this._ValueList, this._ValueList.Length + 1);
                this._ValueList[this._ValueList.Length - 1] = value;
            }
            else
                throw new ArgumentException(SystemMessages.ARGUMENT_EXISTS);
        }

        public object this[string key]
        {
            get
            {
                if (!string.IsNullOrEmpty(key) && 
                    this._ArgumentInfoIndexes.ContainsKey(key))
                {
                    int index = this._ArgumentInfoIndexes[key];

                    if (index < this._ValueList.Length)
                        return this._ValueList[index];
                }

                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(key) || 
                    !this._ArgumentInfoIndexes.ContainsKey(key))
                    throw new ArgumentException(SystemMessages.ARGUMENT_NOTEXISTS);

                int index = this._ArgumentInfoIndexes[key];

                this._ValueList[index] = value;
            }
        }

        public int Count => this._ArgumentInfoIndexes.Count;
    }
}