using System;

namespace Xeora.Web.Service.Workers
{
    public class Task
    {
        private readonly ActionContainer _ActionContainer;

        internal Task(ActionContainer actionContainer) =>
            this._ActionContainer = actionContainer;

        public string Id => this._ActionContainer.Id;
        public Exception Wait() => this._ActionContainer.Wait();
    }
}
