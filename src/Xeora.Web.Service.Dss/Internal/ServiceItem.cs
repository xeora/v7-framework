using System;
using System.Collections.Generic;
using System.Threading;
using Xeora.Web.Exceptions;

namespace Xeora.Web.Service.Dss.Internal
{
    internal class ServiceItem
    {
        private object _Value;
        
        private readonly object _Lock = new object();
        private readonly Queue<string> _LockQueue;

        public ServiceItem(string key, object initialValue = null, string initialLockCode = null)
        {
            this.Key = key;
            this._Value = initialValue;
            
            this._LockQueue = new Queue<string>();
            
            if (!string.IsNullOrEmpty(initialLockCode))
                this._LockQueue.Enqueue(initialLockCode);
        }
        
        public string Key { get; }

        public object Get() => this._Value;

        public void Set(object value, string lockCode = null)
        {
            lock (this._Lock)
            {
                if (this._LockQueue.Count == 0)
                {
                    this._Value = value;
                    return;
                }

                if (string.IsNullOrEmpty(lockCode))
                    throw new KeyLockedException();
                
                string nextLockCode =
                    this._LockQueue.Peek();

                while (string.CompareOrdinal(lockCode, nextLockCode) != 0)
                    Monitor.Wait(this._Lock);

                this._Value = value;
            }
        }

        public string Lock()
        {
            lock (this._Lock)
            {
                string lockCode = 
                    Guid.NewGuid().ToString();
                
                if (this._LockQueue.Count == 0)
                {
                    this._LockQueue.Enqueue(lockCode);
                    return lockCode;
                }
                
                this._LockQueue.Enqueue(lockCode);
                do
                {
                    string nextLockCode =
                        this._LockQueue.Peek();

                    if (string.CompareOrdinal(lockCode, nextLockCode) == 0)
                        break;
                    
                    Monitor.Wait(this._Lock);
                } while (true);

                return lockCode;
            }
        }

        public void Release(string lockCode)
        {
            lock (this._Lock)
            {
                if (this._LockQueue.Count == 0) return;

                do
                {
                    string nextLockCode =
                        this._LockQueue.Peek();
                    
                    if (string.CompareOrdinal(lockCode, nextLockCode) == 0)
                        break;
                                        
                    Monitor.Wait(this._Lock);
                } while (true);

                this._LockQueue.Dequeue();
                
                Monitor.PulseAll(this._Lock);
            }
        }
    }
}
