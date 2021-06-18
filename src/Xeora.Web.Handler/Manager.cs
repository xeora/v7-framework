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
        private static Manager _current;
        public static Manager Current
        {
            get
            {
                Monitor.Enter(Manager.Lock);
                try
                {
                    return Manager._current ?? (Manager._current = new Manager());
                }
                finally
                {
                    Monitor.Exit(Manager.Lock);
                }
            }
        }

        public Basics.IHandler Create(ref IHttpContext context)
        {
            Monitor.Enter(this._RefreshLock);
            try 
            {
                Basics.IHandler handler = 
                    new Xeora(ref context, this._Refresh);
                this._Refresh = false;

                this._Handlers.TryAdd(handler.HandlerId, new Container(ref handler));
                
                return handler;
            }
            finally
            {
                Monitor.Exit(this._RefreshLock);
            }
        }

        public Basics.IHandler Get(string handlerId)
        {
            if (string.IsNullOrEmpty(handlerId))
                return null;

            return !this._Handlers.TryGetValue(handlerId, out Container handlerContainer) 
                    ? null 
                    : handlerContainer.Handler;
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

            try
            {
                if (this._Refresh)
                    return;
                this._Refresh = true;
                
                Basics.Console.Push(
                    string.Empty, "Domains refresh requested!", string.Empty, false, true);
            }
            finally
            {
                Monitor.Exit(this._RefreshLock);
            }
        }
    }
}
