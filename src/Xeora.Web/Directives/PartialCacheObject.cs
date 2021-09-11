using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    internal class PartialCacheObject
    {
        public PartialCacheObject(string cacheId, string content)
        {
            this.CacheId = cacheId;
            this.Content = content;
        }

        public static string CreateUniqueCacheId(int positionId, string cacheIdExtension, PartialCache partialCache, ref Basics.Domain.IDomain instance)
        {
            if (partialCache == null)
                throw new ArgumentNullException(nameof(partialCache));
            
            if (string.IsNullOrEmpty(instance.Languages.Current.Info.Id) || 
                string.IsNullOrEmpty(partialCache.TemplateTree) || positionId == -1)
                throw new Exceptions.ParseException();

            return $"{instance.Languages.Current.Info.Id}_{partialCache.TemplateTree}_{positionId}_{cacheIdExtension}";
        }

        public string CacheId { get; }
        public string Content { get; }
    }
}