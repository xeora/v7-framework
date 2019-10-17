using System;
using System.Collections.Concurrent;
using System.Threading;
using Xeora.Web.Basics.Context;

namespace Xeora.Web.Handler
{
    public class Manager
    {
        private readonly ConcurrentDictionary<string, Container> _Handlers;
        private readonly object _RefreshLock;
        private bool _Refresh;
        
        private Manager()
        {
            this._Handlers = new ConcurrentDictionary<string, Container>();
            this._RefreshLock = new object();

            Basics.Console.Register(keyInfo => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.R)
                    return;

                this.Refresh();
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
                    return Manager._Current ?? (Manager._Current = new Manager());
                }
                finally
                {
                    Monitor.Exit(Manager.Lock);
                }
            }
        }

        public Basics.IHandler Create(ref IHttpContext context)
        {
            Basics.IHandler handler;

            Monitor.Enter(this._RefreshLock);
            try 
            {
                handler = 
                    new XeoraHandler(ref context, this._Refresh);
                this._Refresh = false;
            }
            finally
            {
                Monitor.Exit(this._RefreshLock);
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
        
        public void Refresh()
        {
            if (!Monitor.TryEnter(this._RefreshLock))
                return;

            if (this._Refresh)
                return;

            this._Refresh = true;
            
            Basics.Console.Push(
                string.Empty, "Domains refresh requested!", string.Empty, false, true);

            Monitor.Exit(this._RefreshLock);
        }
    }
}
