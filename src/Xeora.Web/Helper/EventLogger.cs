using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Xeora.Web.Helper
{
    public sealed class EventLogger : IDisposable
    {
        private int _LogCacheLimit = 100;

        private int _FlushCycle = 5;
        private Timer _FlushTimer = null;

        private object _LogCacheSyncObject;
        private SortedDictionary<long, LogObject> _LogCache;

        public EventLogger()
        {
            this._LogCacheSyncObject = new object();
            this._LogCache = new SortedDictionary<long, LogObject>();
        }

        private static EventLogger _Instance = null;
        private static EventLogger Instance
        {
            get
            {
                if (EventLogger._Instance == null)
                    EventLogger._Instance = new EventLogger();

                return EventLogger._Instance;
            }
        }

        public static void Log(System.Exception exception)
        {
            if (exception != null)
                EventLogger.Log(exception.ToString());
        }

        public static void Log(string content)
        {
            try
            {
                EventLogger.Instance.WriteToCache(new LogObject(content));
            }
            catch (System.Exception ex)
            {
                StringBuilder loggingException = new StringBuilder();

                loggingException.AppendLine("-- LOGGING EXCEPTION --");
                loggingException.Append(ex.ToString());
                loggingException.AppendLine();

                loggingException.AppendLine("-- ORIGINAL LOG CONTENT --");
                loggingException.Append(content);
                loggingException.AppendLine();

                Basics.Console.Push("LOGGING ERROR", loggingException.ToString(), false);
            }
        }

        public static void SetFlushCycle(int minute)
        {
            EventLogger.Instance._FlushCycle = minute;

            if (EventLogger.Instance._FlushTimer != null)
            {
                EventLogger.Instance._FlushTimer.Dispose();
                EventLogger.Instance._FlushTimer =
                    new Timer(
                        new TimerCallback(EventLogger.Instance.FlushInternal),
                        null,
                        (EventLogger.Instance._FlushCycle * 60000),
                        0
                    );
            }
        }

        public static void SetFlushCacheLimit(int cacheLimit) =>
            EventLogger.Instance._LogCacheLimit = cacheLimit;

        public static void Flush() =>
            EventLogger.Instance.FlushInternal(null);

        private void WriteToCache(LogObject logObject)
        {
            if (this._FlushTimer == null)
            {
                this._FlushTimer =
                    new Timer(
                        new TimerCallback(this.FlushInternal),
                        null,
                        (this._FlushCycle * 60000),
                        0
                    );
            }

            Monitor.Enter(this._LogCacheSyncObject);
            try
            {
                TimeSpan totalSpan =
                    System.DateTime.Now.Subtract(new System.DateTime(2000, 1, 1, 0, 0, 0));

                this._LogCache.Add(totalSpan.Ticks, logObject);

                // wait to prevent Ticks equality
                Thread.Sleep(1);
            }
            finally
            {
                Monitor.Exit(this._LogCacheSyncObject);
            }

            if (this._LogCache.Count >= this._LogCacheLimit)
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.FlushInternal));
        }

        private void FlushInternal(object state)
        {
            if (this._FlushTimer != null)
            {
                this._FlushTimer.Dispose();
                this._FlushTimer = null;
            }

            List<long> loggedKeys = new List<long>();
            Monitor.Enter(this._LogCacheSyncObject);
            try
            {
                foreach (long key in this._LogCache.Keys)
                {
                    this.WriteToFile(this._LogCache[key]);

                    loggedKeys.Add(key);
                }
            }
            catch (IOException)
            {
                // Possible reason is file in use. Do nothing and let it run again on next time
            }
            catch (System.Exception ex)
            {
                StringBuilder loggingException = new StringBuilder();

                loggingException.AppendLine("-- LOGGING EXCEPTION --");
                loggingException.Append(ex.ToString());
                loggingException.AppendLine();

                Basics.Console.Push("LOGGING ERROR", loggingException.ToString(), false);
            }
            finally
            {
                if (this._LogCache.Count == loggedKeys.Count)
                    this._LogCache.Clear();
                else
                {
                    foreach (long key in loggedKeys)
                    {
                        if (this._LogCache.ContainsKey(key))
                            this._LogCache.Remove(key);
                    }
                }

                Monitor.Exit(this._LogCacheSyncObject);
            }
        }

        private string PrepareOutputFileLocation(System.DateTime logDateTime)
        {
            string outputLocation = 
                Basics.Configurations.Xeora.Application.Main.LoggingPath;

            if (!Directory.Exists(outputLocation))
                Directory.CreateDirectory(outputLocation);

            return Path.Combine(
                outputLocation,
                string.Format(
                    "{0}.log", DateTime.Format(logDateTime, true).ToString())
            );
        }

        private void WriteToFile(LogObject logObject)
        {
            if (logObject == null)
                return;

            StringBuilder sB = new StringBuilder();

            sB.AppendLine("----------------------------------------------------------------------------");
            sB.AppendLine("Event Time --> " + string.Format("{0}.{1}", logObject.DateTime.ToString(), logObject.DateTime.Millisecond));
            sB.AppendLine("----------------------------------------------------------------------------");
            sB.Append(logObject.Content);
            sB.AppendLine();

            Stream logFS = null;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sB.ToString());

                logFS = new FileStream(
                    this.PrepareOutputFileLocation(logObject.DateTime), 
                    FileMode.Append, 
                    FileAccess.Write, 
                    FileShare.ReadWrite
                );
                logFS.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                if (logFS != null)
                    logFS.Close();
            }
        }

        public void Dispose() =>
            this.FlushInternal(null);

        ~EventLogger() =>
            this.Dispose();

        private class LogObject
        {
            public LogObject(string content)
            {
                this.Content = content;
                this.DateTime = System.DateTime.Now;
            }

            public string Content { get; private set; }
            public System.DateTime DateTime { get; private set; }
        }
    }
}