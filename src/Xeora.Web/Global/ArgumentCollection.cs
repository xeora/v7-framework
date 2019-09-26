using System;
using System.Collections.Generic;

namespace Xeora.Web.Global
{
    public class ArgumentCollection
    {
        private readonly object _Lock;
        private readonly Dictionary<string, int> _ArgumentIndices;
        private readonly Dictionary<string, object> _ArgumentValues;

        public ArgumentCollection()
        {
            this._Lock = new object();
            this._ArgumentIndices =
                new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            this._ArgumentValues =
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }
        
        public void Reset()
        {
            lock (this._Lock)
            {
                this._ArgumentIndices.Clear();
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
            
            if (values.Length > this._ArgumentIndices.Count)
                throw new ArgumentOutOfRangeException(SystemMessages.ARGUMENT_KEYVALUELENGTHMATCH);

            lock (this._Lock)
            {
                this._ArgumentValues.Clear();

                foreach (KeyValuePair<string, int> pair in this._ArgumentIndices)
                {
                    if (pair.Value >= values.Length) continue;
                    
                    this._ArgumentValues[pair.Key] = values[pair.Value];
                }
            }
        }

        public void Replace(ArgumentCollection collection)
        {
            if (collection == null)
                return;

            this.Reset();
            
            lock (this._Lock)
            {
                foreach (KeyValuePair<string, int> pair in collection.GetIndices())
                    this._ArgumentIndices[pair.Key] = pair.Value;
                
                foreach (KeyValuePair<string, object> pair in collection.GetValues())
                    this._ArgumentValues[pair.Key] = pair.Value;
            }
        }

        private IEnumerable<KeyValuePair<string, int>> GetIndices()
        {
            List<KeyValuePair<string, int>> indices = 
                new List<KeyValuePair<string, int>>();
            
            lock (this._Lock)
            {
                foreach (KeyValuePair<string, int> pair in this._ArgumentIndices)
                    indices.Add(new KeyValuePair<string, int>(pair.Key, pair.Value));
            }

            return indices.ToArray();
        }
        
        private IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            List<KeyValuePair<string, object>> values = 
                new List<KeyValuePair<string, object>>();
            
            lock (this._Lock)
            {
                foreach (KeyValuePair<string, object> pair in this._ArgumentValues)
                    values.Add(new KeyValuePair<string, object>(pair.Key, pair.Value));
            }

            return values.ToArray();
        }

        public void AppendKey(string key) =>
            this.AppendKeyWithValue(key, null);

        public void AppendKeyWithValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(SystemMessages.ARGUMENT_CANNOTBEEMPTY);
            
            lock (this._Lock)
            {
                if (!this._ArgumentIndices.ContainsKey(key))
                    this._ArgumentIndices[key] = this._ArgumentIndices.Count;

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