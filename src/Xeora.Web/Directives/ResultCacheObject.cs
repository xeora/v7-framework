using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    internal class ResultCacheObject
    {
        public ResultCacheObject(string cacheId, string content)
        {
            this.CacheId = cacheId;
            this.Content = content;
        }

        public static string CreateUniqueCacheId(Guid resultId, Control control, ref Basics.Domain.IDomain instance)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            
            if (string.IsNullOrEmpty(instance.Languages.Current.Info.Id) || 
                string.IsNullOrEmpty(control.TemplateTree) || 
                Equals(resultId, Guid.Empty))
                throw new Exceptions.ParseException();

            return $"{instance.Languages.Current.Info.Id}_{control.TemplateTree}_{resultId}";
        }

        public string CacheId { get; }
        public string Content { get; }
    }
}