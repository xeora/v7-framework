using System;
using System.Text;
using System.Threading;

namespace Xeora.Web.Service.Workers
{
    internal class ActionContainer
    {
        private readonly object _WaitLock = new object();
        private Exception _Exception;
        private bool _Completed;
        
        public ActionContainer(Action<object> action, object state)
        {
            this.Id = Guid.NewGuid().ToString();
            
            this.Action = action;
            this.State = state;
        }

        private Action<object> Action { get; }
        private object State { get; }

        public void Invoke()
        {
            try
            {
                this.Action.Invoke(this.State);
            }
            catch (Exception ex)
            {
                this._Exception = ex;
                
                Basics.Console.Push("ThreadPool Exception...", ex.Message, ex.ToString(), false, true,
                    type: Basics.Console.Type.Error);
            }
            finally
            {
                lock (this._WaitLock)
                {
                    this._Completed = true;
                    Monitor.PulseAll(this._WaitLock);
                }
            }
        }

        public void PrintContainerDetails()
        {
            if (this.State == null)
                return;

            try
            {
                StringBuilder builder = 
                    new StringBuilder();

                string typeResult = 
                    this.State.GetType().GetProperty("Type")?.GetMethod
                        .Invoke(this.State, null).ToString();
                if (!string.IsNullOrEmpty(typeResult)) builder.Append(typeResult);
                
                string nameResult = 
                    this.State.GetType().GetInterface("INameable")?.GetProperty("DirectiveId")?.GetMethod
                        .Invoke(this.State, null).ToString();
                if (!string.IsNullOrEmpty(nameResult)) 
                    builder.AppendFormat("{0}{1}", builder.Length > 0 ? "\n" : string.Empty, nameResult);

                object arguments =
                    this.State.GetType().GetProperty("Arguments")?.GetValue(this.State);
                string argumentsResult =
                    arguments?.GetType().GetMethod("ToString")?.Invoke(arguments, null).ToString();
                if (!string.IsNullOrEmpty(argumentsResult)) 
                    builder.AppendFormat("{0}{1}", builder.Length > 0 ? "\n" : string.Empty, argumentsResult);

                if (builder.Length == 0) return;
                
                Basics.Console.Push("ActionContainer Report", this.Id, $"{builder}", false, true);
            }
            catch
            {
                /* Just handle exceptions */
            }
        }
        
        public string Id { get; }
        
        public Exception Wait()
        {
            lock (this._WaitLock)
            {
                if (this._Completed) return this._Exception;
                
                Monitor.Wait(this._WaitLock);
                return this._Exception;
            }
        }
    }
}
