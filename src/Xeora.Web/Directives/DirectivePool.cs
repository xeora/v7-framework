using System.Collections.Concurrent;

namespace Xeora.Web.Directives
{
    public class DirectivePool
    {
        private readonly ConcurrentDictionary<string, IDirective> _Directives;

        public DirectivePool() =>
            this._Directives = new ConcurrentDictionary<string, IDirective>();

        public void Register(IDirective directive) =>
            this._Directives.AddOrUpdate(directive.UniqueID, directive, (cUniqueID, cController) => directive);

        public void GetInto(string uniqueID, out IDirective directive) =>
            this._Directives.TryGetValue(uniqueID, out directive);

        public void Unregister(string uniqueID) =>
            this._Directives.TryRemove(uniqueID, out IDirective dummy);
    }
}
