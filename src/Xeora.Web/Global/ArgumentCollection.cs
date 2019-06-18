using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ArgumentCollection
    {
        private ConcurrentDictionary<string, int> _ArgumentIndexes;

        public ArgumentCollection()
        {
            this._ArgumentIndexes = 
                new ConcurrentDictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this.Values = new object[] { };
        }

        public void Reset()
        {
            this._ArgumentIndexes.Clear();
            this.Values = new object[] { };
        }

        public void Reset(string[] keys)
        {
            this.Reset();

            if (keys == null)
                return;
            
            foreach (string key in keys)
                this.AppendKey(key);
        }

        public void Reset(object[] values)
        {
            this.Values = new object[this._ArgumentIndexes.Count];

            if (values == null)
                return;
            
            if (values.Length > this.Values.Length)
                throw new ArgumentOutOfRangeException(SystemMessages.ARGUMENT_KEYVALUELENGTHMATCH);

            Array.Copy(values, 0, this.Values, 0, values.Length);
        }

        public void Replace(ArgumentCollection aIC)
        {
            if (aIC == null)
                return;

            this._ArgumentIndexes.Clear();
            foreach (KeyValuePair<string, int> pair in aIC._ArgumentIndexes)
                this._ArgumentIndexes[pair.Key] = pair.Value;

            this.Values = new object[this._ArgumentIndexes.Count];
            Array.Copy(aIC.Values, 0, this.Values, 0, aIC.Values.Length);
        }

        public void AppendKey(string key) =>
            this.AppendKeyWithValue(key, null);

        public void AppendKeyWithValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key) ||
                this._ArgumentIndexes.ContainsKey(key))
            {
                this[key] = value;

                return;
            }
            
            // Add Key
            this._ArgumentIndexes.TryAdd(key, this._ArgumentIndexes.Count);

            // Add Value
            object[] newValues = new object[this._ArgumentIndexes.Count];
            Array.Copy(this.Values, 0, newValues, 0, this.Values.Length);
            newValues[newValues.Length - 1] = value;
            this.Values = newValues;
        }

        public bool ContainsKey(string key) =>
            this._ArgumentIndexes.ContainsKey(key);

        public object this[string key]
        {
            get
            {
                if (!string.IsNullOrEmpty(key) && 
                    this._ArgumentIndexes.ContainsKey(key))
                {
                    int index = this._ArgumentIndexes[key];

                    if (index < this.Values.Length)
                        return this.Values[index];
                }

                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(key) || 
                    !this._ArgumentIndexes.ContainsKey(key))
                    throw new ArgumentException(SystemMessages.ARGUMENT_NOTEXISTS);

                int index = this._ArgumentIndexes[key];

                this.Values[index] = value;
            }
        }

        public object[] Values { get; private set; }

        public int Count => this._ArgumentIndexes.Count;

        public ArgumentCollection Clone()
        {
            ArgumentCollection output = new ArgumentCollection();
            output.Replace(this);

            return output;
        }
    }
}