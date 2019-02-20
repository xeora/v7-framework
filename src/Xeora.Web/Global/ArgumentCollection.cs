using System;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ArgumentCollection
    {
        private Dictionary<string, int> _ArgumentIndexes;
        private object[] _ValueList;

        public ArgumentCollection()
        {
            this._ArgumentIndexes = 
                new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this._ValueList = new object[] { };
        }

        public void Reset()
        {
            this._ArgumentIndexes.Clear();
            this._ValueList = new object[] { };
        }

        public void Reset(string[] keys)
        {
            this._ArgumentIndexes.Clear();
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

        public void Replace(ArgumentCollection aIC)
        {
            if (aIC != null)
            {
                this._ArgumentIndexes = aIC._ArgumentIndexes;
                this._ValueList = aIC._ValueList;
            }
        }

        public void AppendKey(string key) =>
            this.AppendKeyWithValue(key, null);

        public void AppendKeyWithValue(string key, object value)
        {
            if (!string.IsNullOrEmpty(key) && 
                !this._ArgumentIndexes.ContainsKey(key))
            {
                // Add Key
                this._ArgumentIndexes.Add(key, this._ArgumentIndexes.Count);

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
                    this._ArgumentIndexes.ContainsKey(key))
                {
                    int index = this._ArgumentIndexes[key];

                    if (index < this._ValueList.Length)
                        return this._ValueList[index];
                }

                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(key) || 
                    !this._ArgumentIndexes.ContainsKey(key))
                    throw new ArgumentException(SystemMessages.ARGUMENT_NOTEXISTS);

                int index = this._ArgumentIndexes[key];

                this._ValueList[index] = value;
            }
        }

        public int Count => this._ArgumentIndexes.Count;
    }
}