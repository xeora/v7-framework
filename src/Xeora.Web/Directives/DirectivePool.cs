using System.Collections.Concurrent;
using System.Collections.Generic;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public class DirectivePool
    {
        private readonly ConcurrentDictionary<string, IDirective> _Directives;
        private readonly ConcurrentDictionary<string, List<string>> _NamingMap;

        public DirectivePool()
        {
            this._Directives = new ConcurrentDictionary<string, IDirective>();
            this._NamingMap = new ConcurrentDictionary<string, List<string>>();
        }

        public void Register(IDirective directive)
        {
            this._Directives.AddOrUpdate(directive.UniqueId, directive, (cUniqueId, cController) => directive);

            if (!(directive is INamable)) return;

            string directiveId = ((INamable)directive).DirectiveId;

            this._NamingMap.AddOrUpdate(directiveId, new List<string> { directive.UniqueId }, (cKey, cValue) => { cValue.Add(directive.UniqueId); return cValue; });
        }

        public void GetByUniqueId(string uniqueId, out IDirective directive) =>
            this._Directives.TryGetValue(uniqueId, out directive);

        public void GetByDirectiveId(string directiveId, out IEnumerable<IDirective> directives)
        {
            if (!this._NamingMap.TryGetValue(directiveId, out List<string> uniqueIds))
            {
                directives = null;
                return;
            }

            directives = new List<IDirective>();

            foreach (string uniqueId in uniqueIds)
            {
                if (!this._Directives.TryGetValue(uniqueId, out IDirective directive)) continue;
                ((List<IDirective>)directives).Add(directive);
            }
        }

        public void Unregister(string uniqueId)
        {
            this._Directives.TryRemove(uniqueId, out IDirective directive);

            if (!(directive is INamable)) return;

            string directiveId = ((INamable)directive).DirectiveId;

            this._NamingMap.TryGetValue(directiveId, out List<string> uniqueIds);
            uniqueIds?.Remove(uniqueId);
        }
    }
}
