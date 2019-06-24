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
            this._Directives.AddOrUpdate(directive.UniqueID, directive, (cUniqueID, cController) => directive);

            if (!(directive is INamable)) return;

            string directiveID = ((INamable)directive).DirectiveID;

            this._NamingMap.AddOrUpdate(directiveID, new List<string>() { directive.UniqueID }, (cKey, cValue) => { cValue.Add(directive.UniqueID); return cValue; });
        }

        public void GetByUniqueID(string uniqueID, out IDirective directive) =>
            this._Directives.TryGetValue(uniqueID, out directive);

        public void GetByDirectiveID(string directiveID, out IDirective[] directives)
        {
            directives = null;

            if (!this._NamingMap.TryGetValue(directiveID, out List<string> uniqueIDs)) return;

            List<IDirective> _directives = new List<IDirective>();

            foreach (string uniqueID in uniqueIDs)
            {
                if (!this._Directives.TryGetValue(uniqueID, out IDirective directive)) continue;
                _directives.Add(directive);
            }

            directives = _directives.ToArray();
        }

        public void Unregister(string uniqueID)
        {
            this._Directives.TryRemove(uniqueID, out IDirective directive);

            if (!(directive is INamable)) return;

            string directiveID = ((INamable)directive).DirectiveID;

            this._NamingMap.TryGetValue(directiveID, out List<string> uniqueIDs);
            uniqueIDs.Remove(uniqueID);
        }
    }
}
