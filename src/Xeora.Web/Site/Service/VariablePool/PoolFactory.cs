using System;
using System.IO;
using System.Collections.Concurrent;
using Xeora.Web.Basics.Service;
using System.Timers;
using System.Collections.Generic;

namespace Xeora.Web.Site.Service
{
    public class PoolFactory : ConcurrentDictionary<string, IVariablePool>
    {
        private static PoolFactory _Instance = null;

        private int _VariablePoolExpiresInMinutes;
        private string _PoolCacheLocation;

        private Timer _CleanupTimer;

        private PoolFactory(int variablePoolExpiresInMinutes)
        {
            this._VariablePoolExpiresInMinutes = variablePoolExpiresInMinutes;
            this._PoolCacheLocation =
                Path.Combine(
                    Basics.Configurations.Xeora.Application.Main.TemporaryRoot,
                    Basics.Configurations.Xeora.Application.Main.WorkingPath.WorkingPathID,
                    "PoolSessions"
                );

            this.LoadFromDisk();

            this._CleanupTimer = new Timer(variablePoolExpiresInMinutes * 1000);
            this._CleanupTimer.Elapsed += new ElapsedEventHandler(this.Cleanup);
            this._CleanupTimer.Start();

            PoolFactory._Instance = this;
        }

        public static void Initialize(int variablePoolExpiresInMinutes)
        {
            if (PoolFactory._Instance != null)
                return;

            PoolFactory._Instance = new PoolFactory(variablePoolExpiresInMinutes);
        }

        public static void Get(string sessionID, ref IVariablePool variablePool)
        {
            if (PoolFactory._Instance == null)
                throw new Exception.VariablePoolNotReadyException();

            PoolFactory._Instance.ProvideVariablePool(sessionID, ref variablePool);
        }

        public static void Copy(string fromSessionID, string toSessionID)
        {
            if (PoolFactory._Instance == null)
                throw new Exception.VariablePoolNotReadyException();

            PoolFactory._Instance.CopyVariablePool(fromSessionID, toSessionID);
        }

        private void ProvideVariablePool(string sessionID, ref IVariablePool variablePool)
        {
            if (!PoolFactory._Instance.TryGetValue(sessionID, out variablePool))
            {
                variablePool = new VariablePool(sessionID, this._VariablePoolExpiresInMinutes);

                if (!PoolFactory._Instance.TryAdd(sessionID, variablePool))
                    this.ProvideVariablePool(sessionID, ref variablePool);

                return;
            }
        }

        private void CopyVariablePool(string fromSessionID, string toSessionID)
        {
            IVariablePool oldVariablePool = null;
            this.ProvideVariablePool(fromSessionID, ref oldVariablePool);

            IVariablePool newVariablePool = null;
            this.ProvideVariablePool(toSessionID, ref newVariablePool);

            oldVariablePool.CopyInto(ref newVariablePool);
        }

        private void Cleanup(object sender, EventArgs args)
        {
            List<string> deleteSessions = new List<string>();

            foreach (IVariablePool variablePool in base.Values)
            {
                DateTime expireDate = variablePool.LastAccess.AddMinutes(this._VariablePoolExpiresInMinutes);

                if (DateTime.Compare(DateTime.Now, expireDate) > 0)
                {
                    deleteSessions.Add(variablePool.SessionID);

                    continue;
                }

                variablePool.Cleanup();
            }

            foreach (string deleteSessionID in deleteSessions)
            {
                IVariablePool dummy;
                base.TryRemove(deleteSessionID, out dummy);
            }
        }

        private void LoadFromDisk()
        {
            DirectoryInfo poolCacheDI = new DirectoryInfo(this._PoolCacheLocation);

            if (!poolCacheDI.Exists)
                return;

            Stream poolFS;
            foreach (FileInfo vpf in poolCacheDI.GetFiles("*.vpf"))
            {
                poolFS = null;
                try
                {
                    byte[] cache = new byte[vpf.Length];

                    poolFS = vpf.OpenRead();
                    poolFS.Read(cache, 0, cache.Length);

                    IVariablePool result = Helper.Serialization.Binary.DeSerialize<IVariablePool>(cache);

                    if (result == null)
                        continue;

                    base.TryAdd(result.SessionID, result);
                }
                catch (System.Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    if (poolFS != null)
                        poolFS.Close();
                }
            }
        }

        private void FlushToDisk()
        {
            Stream poolFS; string poolFileName;
            foreach (IVariablePool variablePool in base.Values)
            {
                poolFS = null;
                try
                {
                    byte[] vps = Helper.Serialization.Binary.Serialize(variablePool);

                    if (vps == null)
                        continue;

                    poolFileName = Path.Combine(this._PoolCacheLocation, variablePool.SessionID, "vpf");
                    poolFS = new FileStream(poolFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                    poolFS.Write(vps, 0, vps.Length);
                }
                catch (System.Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    if (poolFS != null)
                        poolFS.Close();
                }
            }
        }

        ~PoolFactory()
        {
            this._CleanupTimer.Enabled = false;
            this._CleanupTimer.Dispose();

            this.FlushToDisk();
        }
    }
}
