using System;
using System.Collections.Concurrent;
using System.Threading;
using Xeora.Web.Basics.Context;
using Xeora.Web.Application.Services;

namespace Xeora.Web.Handler
{
    public class Manager
    {
        private readonly ConcurrentDictionary<string, Container> _Handlers;

        private Manager()
        {
            this._Handlers = new ConcurrentDictionary<string, Container>();

            Basics.Console.Register(keyInfo => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.R)
                    return;

                Manager.Refresh();
            });
        }

        private static readonly object Lock = new object();
        private static Manager _Current;
        public static Manager Current
        {
            get
            {
                Monitor.Enter(Manager.Lock);
                try
                {
                    if (Manager._Current == null)
                        Manager._Current = new Manager();
                }
                finally
                {
                    Monitor.Exit(Manager.Lock);
                }

                return Manager._Current;
            }
        }

        public Basics.IHandler Create(ref IHttpContext context)
        {
            // TODO: Pool Variable Expire minutes should me dynamic
            PoolFactory.Initialize(20);

            Basics.IHandler handler;

            Monitor.Enter(Manager.RefreshLock);
            try 
            {
                handler = 
                    new XeoraHandler(ref context, Manager._Refresh);
                Manager._Refresh = false;
            }
            finally
            {
                Monitor.Exit(Manager.RefreshLock);
            }

            this.Add(ref handler);

            return handler;
        }

        public Basics.IHandler Get(string handlerId)
        {
            if (string.IsNullOrEmpty(handlerId))
                return null;

            return !this._Handlers.TryGetValue(handlerId, out Container handlerContainer) 
                    ? null 
                    : handlerContainer.Handler;
        }

        private void Add(ref Basics.IHandler handler)
        {
            if (handler == null)
                return;

            Container container =
                new Container(ref handler);

            while (!this._Handlers.TryAdd(handler.HandlerId, container))
                this.Add(ref handler);
        }

        public void Mark(string handlerId)
        {
            if (!this._Handlers.TryGetValue(handlerId, out Container handlerContainer))
                return;

            handlerContainer.Removable = false;
        }

        public void UnMark(string handlerId)
        {
            if (!this._Handlers.TryGetValue(handlerId, out Container handlerContainer))
                return;

            if (handlerContainer.Removable)
                this._Handlers.TryRemove(handlerId, out handlerContainer);
            else
                handlerContainer.Removable = true;
        }

        private static readonly object RefreshLock = new object();
        private static bool _Refresh;
        internal static void Refresh()
        {
            if (!Monitor.TryEnter(Manager.RefreshLock))
                return;

            if (Manager._Refresh)
                return;

            Manager._Refresh = true;
            
            Basics.Console.Push(
                string.Empty, "Domains refresh requested!", string.Empty, false, true);

            Monitor.Exit(Manager.RefreshLock);
        }
    }
}
