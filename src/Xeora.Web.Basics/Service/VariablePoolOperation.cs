using System;
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
        private static object _Lock = new object();
        private static IVariablePool _Cache;

        private readonly string _SessionId;
        private readonly string _KeyId;
        private readonly string _SessionKeyId;

        public VariablePoolOperation(string sessionId, string keyId)
        {
            Monitor.Enter(VariablePoolOperation._Lock);
            try
            {
                if (VariablePoolOperation._Cache == null)
                {
                    try
                    {
                        VariablePoolOperation._Cache =
                            (IVariablePool)TypeCache.Current.RemoteInvoke.InvokeMember("GetVariablePool", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { sessionId, keyId });
                    }
                    catch (Exception ex)
                    {
                        throw new TargetInvocationException("Communication Error! Variable Pool is not accessable...", ex);
                    }
                }
            }
            finally
            {
                Monitor.Exit(VariablePoolOperation._Lock);
            }

            this._SessionId = sessionId;
            this._KeyId = keyId;
            this._SessionKeyId = string.Format("{0}_{1}", sessionId, keyId);
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

            if (objectValue is T)
                return (T)objectValue;

            return default(T);
        }

        public void Transfer(string fromSessionId) =>
            this.TransferRegistrations(string.Format("{0}_{1}", fromSessionId, this._KeyId));

        private object GetVariableFromPool(string name)
        {
            object rObject = VariablePoolPreCache.GetCachedVariable(this._SessionKeyId, name);

            if (rObject == null)
            {
                byte[] serializedValue = VariablePoolOperation._Cache.Get(name);

                if (serializedValue != null)
                {
                    Stream forStream = null;

                    try
                    {
                        BinaryFormatter binFormater = new BinaryFormatter();
                        binFormater.Binder = new OverrideBinder();

                        forStream = new MemoryStream(serializedValue);

                        rObject = binFormater.Deserialize(forStream);

                        VariablePoolPreCache.CacheVariable(this._SessionKeyId, name, rObject);
                    }
                    catch (Exception)
                    {
                        // Just Handle Exceptions
                    }
                    finally
                    {
                        forStream?.Close();
                    }
                }
            }

            return rObject;
        }

        private void RegisterVariableToPool(string name, object value)
        {
            VariablePoolPreCache.CleanCachedVariables(this._SessionKeyId, name);

            byte[] serializedValue;
            Stream forStream = null;
            try
            {
                forStream = new MemoryStream();

                BinaryFormatter binFormater = new BinaryFormatter();
                binFormater.Serialize(forStream, value);

                serializedValue = ((MemoryStream)forStream).ToArray();
            }
            catch (Exception)
            {
                serializedValue = new byte[] { };
            }
            finally
            {
                forStream?.Close();
            }

            VariablePoolOperation._Cache.Set(name, serializedValue);
        }

        private void UnRegisterVariableFromPool(string name)
        {
            VariablePoolPreCache.CleanCachedVariables(this._SessionKeyId, name);

            // Unregister Variable From Pool Immidiately. 
            // Otherwise it will cause cache reload in the same domain call
            VariablePoolOperation._Cache.Set(name, null);
        }

        private void TransferRegistrations(string fromSessionId)
        {
            try
            {
                VariablePoolOperation._Cache =
                    (IVariablePool)TypeCache.Current.RemoteInvoke.InvokeMember("TransferVariablePool", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { this._KeyId, fromSessionId, this._SessionId });
            }
            catch (Exception ex)
            {
                throw new TargetInvocationException("Communication Error! Variable Pool is not accessable...", ex);
            }
        }

        private byte[] SerializeNameValuePairs(ConcurrentDictionary<string, object> nameValuePairs)
        {
            if (nameValuePairs == null)
                return new byte[] { };

            SerializableDictionary serializableDictionary = new SerializableDictionary();

            Stream forStream = null;
            foreach (string variableName in nameValuePairs.Keys)
            {
                forStream = null;
                try
                {
                    if (nameValuePairs.TryGetValue(variableName, out object variableValue))
                    {
                        forStream = new MemoryStream();

                        BinaryFormatter binFormater = new BinaryFormatter();
                        binFormater.Serialize(forStream, variableValue);

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
                    forStream?.Close();
                }
            }

            forStream = null;
            try
            {
                forStream = new MemoryStream();

                BinaryFormatter binFormater = new BinaryFormatter();
                binFormater.Serialize(forStream, serializableDictionary);

                return ((MemoryStream)forStream).ToArray();
            }
            catch (Exception)
            {
                return new byte[] { };
            }
            finally
            {
                forStream?.Close();
            }
        }

        // This class required to eliminate the mass request to VariablePool.
        // VariablePool registration requires serialization...
        // Use PreCache for only read keys do not use for variable registration!
        // It is suitable for repeating requests...
        private class VariablePoolPreCache
        {
            private static object _Lock = new object();
            private static ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _VariablePreCache = null;
            public static ConcurrentDictionary<string, ConcurrentDictionary<string, object>> VariablePreCache
            {
                get
                {
                    Monitor.Enter(VariablePoolPreCache._Lock);
                    try
                    {
                        if (VariablePoolPreCache._VariablePreCache == null)
                            VariablePoolPreCache._VariablePreCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>();
                    }
                    finally
                    {
                        Monitor.Exit(VariablePoolPreCache._Lock);
                    }

                    return VariablePoolPreCache._VariablePreCache;
                }
            }

            public static object GetCachedVariable(string sessionKeyId, string name)
            {
                if (VariablePoolPreCache.VariablePreCache.TryGetValue(sessionKeyId, out ConcurrentDictionary<string, object> nameValuePairs))
                {
                    if (nameValuePairs.TryGetValue(name, out object value) && value != null)
                        return value;
                }

                return null;
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
                    nameValuePairs.TryRemove(name, out object dummy);
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

                public string Name { get; private set; }
                public byte[] Value { get; private set; }
            }
        }
    }
}