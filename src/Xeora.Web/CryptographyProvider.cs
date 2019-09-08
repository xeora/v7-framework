using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xeora.Web
{
    public class CryptographyProvider
    {
        private ConcurrentDictionary<string, Basics.ICryptography> _Cryptos;
        private ConcurrentDictionary<string, DateTime> _Timeouts;
        private readonly object _GetLock = new object();
        
        private const short PRUNE_INTERVAL = 10 * 60; // 10 minutes
        private readonly object _PruneLock;

        private static CryptographyProvider _Current;
        private static readonly object ProviderLock = 
            new object();
        
        public static CryptographyProvider Current
        {
            get
            {
                if (CryptographyProvider._Current != null)
                    return CryptographyProvider._Current;
                
                Monitor.Enter(CryptographyProvider.ProviderLock);
                try
                {
                    CryptographyProvider._Current = 
                        new CryptographyProvider();
                    return CryptographyProvider._Current;
                }
                finally
                {
                    Monitor.Exit(CryptographyProvider.ProviderLock);
                }
            }
        }

        private CryptographyProvider()
        {
            this._Cryptos = new ConcurrentDictionary<string, Basics.ICryptography>();
            this._Timeouts = new ConcurrentDictionary<string, DateTime>();
         
            this._PruneLock = new object();
            
            System.Timers.Timer pruneTimer = 
                new System.Timers.Timer(CryptographyProvider.PRUNE_INTERVAL * 1000);
            pruneTimer.Elapsed += this.PruneCryptos;
            pruneTimer.Start();
        }

        public Basics.ICryptography Get(string cryptoId)
        {
            Monitor.Enter(this._GetLock);
            try
            {
                if (this._Cryptos.TryGetValue(cryptoId, out Basics.ICryptography crypto))
                    return crypto;
            
                crypto = 
                    new Cryptography(cryptoId);
                this._Cryptos.TryAdd(cryptoId, crypto);

                return crypto;
            }
            finally
            {
                this.Ping(cryptoId);
                Monitor.Exit(this._GetLock);
            }
        }

        public void Ping(string cryptoId) =>
            this._Timeouts.AddOrUpdate(cryptoId, DateTime.Now, (k, v) => DateTime.Now);

        private void PruneCryptos(object sender, EventArgs args)
        {
            if (!Monitor.TryEnter(this._PruneLock))
                return;
            
            try
            {            
                string[] cryptoIds = 
                    new string[this._Timeouts.Keys.Count];
                this._Timeouts.Keys.CopyTo(cryptoIds, 0);

                foreach (string cryptoId in cryptoIds)
                {
                    if (!this._Timeouts.TryGetValue(cryptoId, out DateTime createDateTime))
                        continue;

                    createDateTime =
                        createDateTime.AddMinutes(Basics.Configurations.Xeora.Session.Timeout);

                    if (DateTime.Compare(createDateTime, DateTime.Now) > 0) continue;

                    this.Destroy(cryptoId);
                }
            }
            finally
            {
                Monitor.Exit(this._PruneLock);
            }
        }

        private void Destroy(string cryptoId)
        {
            this._Cryptos.TryRemove(cryptoId, out _);
            this._Timeouts.TryRemove(cryptoId, out _);
        }
    }
}
