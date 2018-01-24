using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Xeora.Web.Controller.Directive
{
    public class PartialCache : DirectiveWithChildren, IInstanceRequires
    {
        private string[] _IDAccessTree;
        private string _CacheID;

        public event InstanceHandler InstanceRequested;

        public PartialCache(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.PartialCache, contentArguments)
        { }

        public override void Render(string requesterUniqueID)
        {
            Basics.Domain.IDomain instance = null;
            InstanceRequested?.Invoke(ref instance);

            this._IDAccessTree = instance.IDAccessTree;
            this._CacheID =
                CacheObject.ProvideUniqueCacheID(this, ref instance);

            CacheObject cacheObject = null;
            CachePool.Current.Get(this._IDAccessTree, this._CacheID, out cacheObject);

            if (cacheObject != null)
            {
                this.RenderedValue = cacheObject.Content;

                return;
            }

            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            string blockContent = contentDescription.Parts[0];

            this.Parse(blockContent);
            base.Render(requesterUniqueID);
        }

        public override void Build()
        {
            if (this.IsRendered)
                return;

            base.Build();

            CacheObject cacheObject = new CacheObject(this._CacheID, this.RenderedValue);
            CachePool.Current.AddOrUpdate(this._IDAccessTree, cacheObject);
        }

        public static void ClearCache(string[] domainIDAccessTree)
        {
            CachePool.Current.Reset(domainIDAccessTree);
        }

        private class CachePool
        {
            private ConcurrentDictionary<string[], ConcurrentDictionary<string, CacheObject>> _PartialCaches = null;

            private CachePool()
            {
                this._PartialCaches = new ConcurrentDictionary<string[], ConcurrentDictionary<string, CacheObject>>();
            }

            private static CachePool _Current = null;
            public static CachePool Current
            {
                get
                {
                    if (CachePool._Current == null)
                        CachePool._Current = new CachePool();

                    return CachePool._Current;
                }
            }

            public void AddOrUpdate(string[] domainIDAccessTree, CacheObject cacheObject)
            {
                ConcurrentDictionary<string, CacheObject> cacheObjects;

                if (!this._PartialCaches.TryGetValue(domainIDAccessTree, out cacheObjects))
                {
                    cacheObjects = new ConcurrentDictionary<string, CacheObject>();

                    if (!this._PartialCaches.TryAdd(domainIDAccessTree, cacheObjects))
                    {
                        this.AddOrUpdate(domainIDAccessTree, cacheObject);

                        return;
                    }
                }

                cacheObjects.AddOrUpdate(cacheObject.CacheID, cacheObject, (cCacheID, cCacheObject) => cacheObject);
            }

            public void Get(string[] domainIDAccessTree, string cacheID, out CacheObject cacheObject)
            {
                cacheObject = null;

                ConcurrentDictionary<string, CacheObject> cacheObjects;
                if (!this._PartialCaches.TryGetValue(domainIDAccessTree, out cacheObjects))
                    return;

                cacheObjects.TryGetValue(cacheID, out cacheObject);
            }

            public void Reset(string[] domainIDAccessTree)
            {
                ConcurrentDictionary<string, CacheObject> dummy;
                this._PartialCaches.TryRemove(domainIDAccessTree, out dummy);
            }
        }

        private class CacheObject
        {
            private static Regex _PositionRegEx =
                new Regex("PC~(?<PositionID>\\d+)\\:\\{", RegexOptions.Compiled);

            public CacheObject(string cacheID, string content)
            {
                this.CacheID = cacheID;
                this.Content = content;
                this.Date = DateTime.Now;
            }

            public static string ProvideUniqueCacheID(PartialCache partialCache, ref Basics.Domain.IDomain instance)
            {
                if (partialCache == null)
                    throw new NullReferenceException("PartialCache Parameter must not be null!");

                string serviceFullPath = string.Empty;

                IController workingObject = partialCache.Parent;
                do
                {
                    if (workingObject is Template)
                    {
                        serviceFullPath = ((Template)workingObject).ControlID;

                        break;
                    }

                    workingObject = workingObject.Parent;
                } while (workingObject != null);

                int positionID = -1;
                Match matchMI = CacheObject._PositionRegEx.Match(partialCache.Value);

                if (matchMI.Success)
                    int.TryParse(matchMI.Result("${PositionID}"), out positionID);

                if (string.IsNullOrEmpty(instance.Languages.Current.Info.ID) || string.IsNullOrEmpty(serviceFullPath) || positionID == -1)
                    throw new Exception.ParseException();

                return string.Format("{0}_{1}_{2}", instance.Languages.Current.Info.ID, serviceFullPath, positionID);
            }

            public string CacheID { get; private set; }
            public string Content { get; private set; }
            public DateTime Date { get; private set; }
        }
    }
}