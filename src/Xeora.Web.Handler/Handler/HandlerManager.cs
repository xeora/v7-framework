using System;
using System.Collections.Concurrent;
using System.Threading;
using Xeora.Web.Basics.Context;
using Xeora.Web.Application.Services;

namespace Xeora.Web.Handler
{
    public class HandlerManager
    {
        private readonly ConcurrentDictionary<string, HandlerContainer> _Handlers;

        private HandlerManager()
        {
            this._Handlers = new ConcurrentDictionary<string, HandlerContainer>();

            Basics.Console.Register((keyInfo) => {
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == 0 || keyInfo.Key != ConsoleKey.R)
                    return;

                HandlerManager.Refresh();

                Basics.Console.Push(
                    string.Empty, "Domains refresh requested!", string.Empty, false);
            });
        }

        private static object _Lock = new object();
        private static HandlerManager _Current = null;
        public static HandlerManager Current
        {
            get
            {
                Monitor.Enter(HandlerManager._Lock);
                try
                {
                    if (HandlerManager._Current == null)
                        HandlerManager._Current = new HandlerManager();
                }
                finally
                {
                    Monitor.Exit(HandlerManager._Lock);
                }

                return HandlerManager._Current;
            }
        }

        public Basics.IHandler Create(ref IHttpContext context)
        {
            // TODO: Pool Variable Expire minutes should me dynamic
            PoolFactory.Initialize(20);

            Basics.IHandler handler;

            Monitor.Enter(HandlerManager._RefreshLock);
            try 
            {
                handler = 
                    new XeoraHandler(ref context, HandlerManager._Refresh);
                HandlerManager._Refresh = false;
            }
            finally
            {
                Monitor.Exit(HandlerManager._RefreshLock);
            }

            this.Add(ref handler);

            return handler;
        }

        public Basics.IHandler Get(string handlerId)
        {
            if (string.IsNullOrEmpty(handlerId))
                return null;

            if (!this._Handlers.TryGetValue(handlerId, out HandlerContainer handlerContainer))
                return null;

            return handlerContainer.Handler;
        }

        private void Add(ref Basics.IHandler handler)
        {
            if (handler == null)
                return;

            HandlerContainer handlerContainer =
                new HandlerContainer(ref handler);

            if (!this._Handlers.TryAdd(handler.HandlerId, handlerContainer))
                this.Add(ref handler);
        }

        public void Mark(string handlerId)
        {
            if (!this._Handlers.TryGetValue(handlerId, out HandlerContainer handlerContainer))
                return;

            handlerContainer.Removable = false;
        }

        public void UnMark(string handlerId)
        {
            if (!this._Handlers.TryGetValue(handlerId, out HandlerContainer handlerContainer))
                return;

            if (handlerContainer.Removable)
                this._Handlers.TryRemove(handlerId, out handlerContainer);
            else
                handlerContainer.Removable = true;
        }

        private static object _RefreshLock = new object();
        private static bool _Refresh = false;
        private static void Refresh()
        {
            if (!Monitor.TryEnter(HandlerManager._RefreshLock))
                return;

            if (HandlerManager._Refresh)
                return;

            HandlerManager._Refresh = true;

            Monitor.Exit(HandlerManager._RefreshLock);
        }
    }
}
