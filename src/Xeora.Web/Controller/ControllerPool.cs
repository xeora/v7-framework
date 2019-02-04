using System.Collections.Concurrent;

namespace Xeora.Web.Controller
{
    public class ControllerPool
    {
        private readonly ConcurrentDictionary<string, IController> _Controllers;

        public ControllerPool() =>
            this._Controllers = new ConcurrentDictionary<string, IController>();

        public void Register(IController controller) =>
            this._Controllers.AddOrUpdate(controller.UniqueID, controller, (cUniqueID, cController) => controller);

        public void GetInto(string uniqueID, out IController controller) =>
            this._Controllers.TryGetValue(uniqueID, out controller);

        public void Unregister(string uniqueID) =>
            this._Controllers.TryRemove(uniqueID, out IController dummy);
    }
}
