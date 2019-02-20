using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    internal class PartialCacheObject
    {
        public PartialCacheObject(string cacheID, string content)
        {
            this.CacheID = cacheID;
            this.Content = content;
            this.Date = DateTime.Now;
        }

        public static string CreateUniqueCacheID(int positionID, PartialCache partialCache, ref Basics.Domain.IDomain instance)
        {
            if (partialCache == null)
                throw new ArgumentNullException(nameof(partialCache));

            string serviceFullPath = string.Empty;

            IDirective workingObject = partialCache.Parent;
            do
            {
                if (workingObject is Template)
                {
                    serviceFullPath = ((Template)workingObject).DirectiveID;

                    break;
                }

                workingObject = workingObject.Parent;
            } while (workingObject != null);

            if (string.IsNullOrEmpty(instance.Languages.Current.Info.ID) || string.IsNullOrEmpty(serviceFullPath) || positionID == -1)
                throw new Exception.ParseException();

            return string.Format("{0}_{1}_{2}", instance.Languages.Current.Info.ID, serviceFullPath, positionID);
        }

        public string CacheID { get; private set; }
        public string Content { get; private set; }
        public DateTime Date { get; private set; }
    }
}