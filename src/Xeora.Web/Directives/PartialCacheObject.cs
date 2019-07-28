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
            this.Date = DateTime.Now;
        }

        public static string CreateUniqueCacheId(int positionId, PartialCache partialCache, ref Basics.Domain.IDomain instance)
        {
            if (partialCache == null)
                throw new ArgumentNullException(nameof(partialCache));

            string serviceFullPath = string.Empty;

            IDirective workingObject = partialCache.Parent;
            do
            {
                if (workingObject is Template template)
                {
                    serviceFullPath = template.DirectiveId;
                    break;
                }

                workingObject = workingObject.Parent;
            } while (workingObject != null);

            if (string.IsNullOrEmpty(instance.Languages.Current.Info.Id) || string.IsNullOrEmpty(serviceFullPath) || positionId == -1)
                throw new Exception.ParseException();

            return $"{instance.Languages.Current.Info.Id}_{serviceFullPath}_{positionId}";
        }

        public string CacheId { get; }
        public string Content { get; }
        public DateTime Date { get; }
    }
}