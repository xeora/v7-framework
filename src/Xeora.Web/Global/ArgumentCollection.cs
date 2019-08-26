using System;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ArgumentCollection
    {
        private readonly object _Lock;
        private readonly Dictionary<string, int> _ArgumentIndexes;
        private readonly Dictionary<string, object> _ArgumentValues;

        public ArgumentCollection()
        {
            this._Lock = new object();
            this._ArgumentIndexes =
                new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this._ArgumentValues =
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }
        
        public void Reset()
        {
            lock (this._Lock)
            {
                this._ArgumentIndexes.Clear();
                this._ArgumentValues.Clear();
            }
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
            if (values == null)
                return;
            
            if (values.Length > this._ArgumentIndexes.Count)
                throw new ArgumentOutOfRangeException(SystemMessages.ARGUMENT_KEYVALUELENGTHMATCH);

            lock (this._Lock)
            {
                this._ArgumentValues.Clear();

                foreach (KeyValuePair<string, int> pair in this._ArgumentIndexes)
                {
                    if (pair.Value >= values.Length) continue;
                    
                    this._ArgumentValues[pair.Key] = values[pair.Value];
                }
            }
        }

        public void Replace(ArgumentCollection aC)
        {
            if (aC == null)
                return;

            this.Reset();
            
            lock (this._Lock)
            {
                foreach (KeyValuePair<string, int> pair in aC._ArgumentIndexes)
                    this._ArgumentIndexes[pair.Key] = pair.Value;
                
                foreach (KeyValuePair<string, object> pair in aC._ArgumentValues)
                    this._ArgumentValues[pair.Key] = pair.Value;
            }
        }

        public void AppendKey(string key) =>
            this.AppendKeyWithValue(key, null);

        public void AppendKeyWithValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(SystemMessages.ARGUMENT_CANNOTBEEMPTY);
            
            lock (this._Lock)
            {
                if (!this._ArgumentIndexes.ContainsKey(key))
                    this._ArgumentIndexes[key] = this._ArgumentIndexes.Count;

                this._ArgumentValues[key] = value;
            }
        }
        
        public object this[string key]
        {
            get
            {
                lock (this._Lock)
                {
                    if (string.IsNullOrEmpty(key) || !this._ArgumentValues.ContainsKey(key)) return null;
                    return this._ArgumentValues[key];
                }
            }
        }

        public ArgumentCollection Clone()
        {
            ArgumentCollection output = 
                new ArgumentCollection();
            output.Replace(this);

            return output;
        }
    }
}