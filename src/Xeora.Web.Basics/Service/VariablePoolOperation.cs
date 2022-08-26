﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web.Basics.Service
{
    public sealed class VariablePoolOperation
    {
        private static readonly object Lock = new object();

        private readonly string _SessionId;
        private readonly string _KeyId;
        private readonly string _SessionKeyId;
        private readonly IVariablePool _VariablePool;

        public VariablePoolOperation(string sessionId, string keyId)
        {
            Monitor.Enter(VariablePoolOperation.Lock);
            try
            {
                this._VariablePool =
                    Helpers.Negotiator.GetVariablePool(sessionId, keyId);
            }
            catch (Exception ex)
            {
                throw new TargetInvocationException("Communication Error! Variable Pool is not accessible...", ex);
            }
            finally
            {
                Monitor.Exit(VariablePoolOperation.Lock);
            }

            this._SessionId = sessionId;
            this._KeyId = keyId;
            this._SessionKeyId = $"{sessionId}_{keyId}";
        }

        public void Set(string name, object value)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Length > 128)
                throw new ArgumentOutOfRangeException(nameof(name), "Key must not be longer than 128 characters!");

            if (value == null)
            {
                this.UnRegisterVariableFromPool(name);

                return;
            }

            this.RegisterVariableToPool(name, value);
        }

        public object Get(string name) =>
            this.GetVariableFromPool(name);

        public T Get<T>(string name)
        {
            object objectValue = this.Get(name);

            if (objectValue is T value)
                return value;

            return default;
        }

        /*public void Transfer(string fromSessionId) =>
            this.TransferRegistrations($"{fromSessionId}_{this._KeyId}");*/

        private object GetVariableFromPool(string name)
        {
            object rObject = 
                VariablePoolPreCache.GetCachedVariable(this._SessionKeyId, name);

            if (rObject != null) return rObject;
            
            byte[] serializedValue = 
                this._VariablePool.Get(name);

            if (serializedValue == null) return null;
            
            Stream forStream = null;
            try
            {
                BinaryFormatter binFormatter = 
                    new BinaryFormatter {Binder = new OverrideBinder()};

                forStream = new MemoryStream(serializedValue);

                rObject = binFormatter.Deserialize(forStream);

                VariablePoolPreCache.CacheVariable(this._SessionKeyId, name, rObject);

                return rObject;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                forStream?.Dispose();
            }
        }

        private void RegisterVariableToPool(string name, object value)
        {
            VariablePoolPreCache.CleanCachedVariables(this._SessionKeyId, name);

            byte[] serializedValue;
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream();

                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(forStream, value);

                serializedValue = ((MemoryStream)forStream).ToArray();
            }
            catch (Exception e)
            {
                Console.Push(
                    "VP Serializer Exception...", 
                    e.Message, 
                    e.ToString(), 
                    false, 
                    true,
                    type: Console.Type.Error);
                
                serializedValue = new byte[] { };
            }
            finally
            {
                forStream?.Dispose();
            }

            this._VariablePool.Set(name, serializedValue);
        }

        private void UnRegisterVariableFromPool(string name)
        {
            VariablePoolPreCache.CleanCachedVariables(this._SessionKeyId, name);

            // Unregister Variable From Pool Immediately. 
            // Otherwise it will cause cache reload in the same domain call
            this._VariablePool.Set(name, null);
        }

        /*private void TransferRegistrations(string fromSessionId)
        {
            try
            {
                TypeCache.Current.Negotiator.TransferVariablePool(this._KeyId, fromSessionId, this._SessionId);
            }
            catch (Exception ex)
            {
                throw new TargetInvocationException("Communication Error! Variable Pool is not accessible...", ex);
            }
        }*/

        private byte[] SerializeNameValuePairs(ConcurrentDictionary<string, object> nameValuePairs)
        {
            if (nameValuePairs == null)
                return new byte[] { };

            SerializableDictionary serializableDictionary = new SerializableDictionary();

            Stream forStream;
            foreach (string variableName in nameValuePairs.Keys)
            {
                forStream = null;
                try
                {
                    if (nameValuePairs.TryGetValue(variableName, out object variableValue))
                    {
                        forStream = new MemoryStream();

                        BinaryFormatter binFormatter = new BinaryFormatter();
                        binFormatter.Serialize(forStream, variableValue);

                        byte[] serializedValue = ((MemoryStream)forStream).ToArray();

                        serializableDictionary.Add(new SerializableDictionary.SerializableKeyValuePair(variableName, serializedValue));
                    }
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    forStream?.Dispose();
                }
            }

            forStream = null;
            try
            {
                forStream = new MemoryStream();

                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(forStream, serializableDictionary);

                return ((MemoryStream)forStream).ToArray();
            }
            catch (Exception)
            {
                return new byte[] { };
            }
            finally
            {
                forStream?.Dispose();
            }
        }

        // This class required to eliminate the mass request to VariablePool.
        // VariablePool registration requires serialization...
        // Use PreCache for only read keys do not use for variable registration!
        // It is suitable for repeating requests...
        private static class VariablePoolPreCache
        {
            private static readonly object Lock = new object();
            private static ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _variablePreCache;
            private static ConcurrentDictionary<string, ConcurrentDictionary<string, object>> VariablePreCache
            {
                get
                {
                    Monitor.Enter(VariablePoolPreCache.Lock);
                    try
                    {
                        return VariablePoolPreCache._variablePreCache ?? (VariablePoolPreCache._variablePreCache =
                                   new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>());
                    }
                    finally
                    {
                        Monitor.Exit(VariablePoolPreCache.Lock);
                    }
                }
            }

            public static object GetCachedVariable(string sessionKeyId, string name)
            {
                if (!VariablePoolPreCache.VariablePreCache.TryGetValue(sessionKeyId,
                    out ConcurrentDictionary<string, object> nameValuePairs)) return null;
                
                return nameValuePairs.TryGetValue(name, out object value) ? value : null;
            }

            public static void CacheVariable(string sessionKeyId, string name, object value)
            {
                if (!VariablePoolPreCache.VariablePreCache.TryGetValue(sessionKeyId, out ConcurrentDictionary<string, object> nameValuePairs))
                {
                    nameValuePairs = new ConcurrentDictionary<string, object>();

                    if (!VariablePoolPreCache.VariablePreCache.TryAdd(sessionKeyId, nameValuePairs))
                    {
                        VariablePoolPreCache.CacheVariable(sessionKeyId, name, value);

                        return;
                    }
                }

                if (value == null)
                    nameValuePairs.TryRemove(name, out value);
                else
                    nameValuePairs.AddOrUpdate(name, value, (cName, cValue) => value);
            }

            public static void CleanCachedVariables(string sessionKeyId, string name)
            {
                if (VariablePoolPreCache.VariablePreCache.TryGetValue(sessionKeyId, out ConcurrentDictionary<string, object> nameValuePairs))
                    nameValuePairs.TryRemove(name, out _);
            }
        }

        [Serializable]
        public class SerializableDictionary : List<SerializableDictionary.SerializableKeyValuePair>
        {
            [Serializable]
            public class SerializableKeyValuePair
            {
                public SerializableKeyValuePair(string name, byte[] value)
                {
                    this.Name = name;
                    this.Value = value;
                }

                public string Name { get; }
                public byte[] Value { get; }
            }
        }
    }
}