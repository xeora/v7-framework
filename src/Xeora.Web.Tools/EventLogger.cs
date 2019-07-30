using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Xeora.Web.Tools
{
    public sealed class EventLogger : IDisposable
    {
        private int _LogCacheLimit = 100;

        private int _FlushCycle = 5;
        private Timer _FlushTimer;

        private readonly object _LogCacheSyncObject;
        private readonly SortedDictionary<long, LogObject> _LogCache;

        public EventLogger()
        {
            this._LogCacheSyncObject = new object();
            this._LogCache = new SortedDictionary<long, LogObject>();
        }

        private static readonly object Lock = new object();
        private static EventLogger _Current;
        private static EventLogger Current
        {
            get
            {
                Monitor.Enter(EventLogger.Lock);
                try
                {
                    if (EventLogger._Current == null)
                        EventLogger._Current = new EventLogger();
                }
                finally
                {
                    Monitor.Exit(EventLogger.Lock);
                }

                return EventLogger._Current;
            }
        }

        public static void Log(Exception exception)
        {
            if (exception != null)
                EventLogger.Log(exception.ToString());
        }

        public static void Log(string content)
        {
            try
            {
                EventLogger.Current.WriteToCache(new LogObject(content));
            }
            catch (Exception ex)
            {
                StringBuilder loggingException = new StringBuilder();

                loggingException.AppendLine("-- LOGGING EXCEPTION --");
                loggingException.Append(ex);
                loggingException.AppendLine();

                loggingException.AppendLine("-- ORIGINAL LOG CONTENT --");
                loggingException.Append(content);
                loggingException.AppendLine();

                Basics.Console.Push("LOGGING ERROR", string.Empty, loggingException.ToString(), false, type: Basics.Console.Type.Error);
            }
        }

        public static void SetFlushCycle(int minute)
        {
            EventLogger.Current._FlushCycle = minute;

            if (EventLogger.Current._FlushTimer == null) return;
            
            EventLogger.Current._FlushTimer.Dispose();
            EventLogger.Current._FlushTimer =
                new Timer(
                    EventLogger.Current.FlushInternal,
                    null,
                    (EventLogger.Current._FlushCycle * 60000),
                    0
                );
        }

        public static void SetFlushCacheLimit(int cacheLimit) =>
            EventLogger.Current._LogCacheLimit = cacheLimit;

        public static void Flush() =>
            EventLogger.Current.FlushInternal(null);

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
                ThreadPool.QueueUserWorkItem(this.FlushInternal);
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
            catch (Exception ex)
            {
                StringBuilder loggingException = new StringBuilder();

                loggingException.AppendLine("-- LOGGING EXCEPTION --");
                loggingException.Append(ex.ToString());
                loggingException.AppendLine();

                Basics.Console.Push("LOGGING ERROR", string.Empty, loggingException.ToString(), false, type: Basics.Console.Type.Error);
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
                $"{DateTime.Format(logDateTime, true).ToString()}.log"
            );
        }

        private void WriteToFile(LogObject logObject)
        {
            if (logObject == null)
                return;

            StringBuilder sB = new StringBuilder();

            sB.AppendLine("----------------------------------------------------------------------------");
            sB.AppendLine("Event Time --> " + $"{logObject.DateTime}.{logObject.DateTime.Millisecond}");
            sB.AppendLine("----------------------------------------------------------------------------");
            sB.Append(logObject.Content);
            sB.AppendLine();

            Stream logStream = null;
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sB.ToString());

                logStream = new FileStream(
                    this.PrepareOutputFileLocation(logObject.DateTime), 
                    FileMode.Append, 
                    FileAccess.Write, 
                    FileShare.ReadWrite
                );
                logStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                logStream?.Close();
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

            public string Content { get; }
            public System.DateTime DateTime { get; }
        }
    }
}