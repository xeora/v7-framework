using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Property : Directive
    {
        private readonly string _RawValue;
        private readonly bool _CanAsync;

        public Property(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Property, arguments)
        {
            this._RawValue = rawValue;
            this._CanAsync = true;

            if (string.IsNullOrEmpty(this._RawValue))
                return;
            
            switch (this._RawValue)
            {
                case "DomainContents":
                case "PageRenderDuration":
                    break;
                default:
                    switch (this._RawValue[0])
                    {
                        case '^':
                        case '~':
                        case '-':
                        case '+':
                        case '=':
                        case '#':
                            break;
                        case '@':
                            switch (this._RawValue[1])
                            {
                                case '-':
                                case '#':
                                    break;
                                default:
                                    this._CanAsync = false;

                                    break;
                            }

                            break;
                        default:
                            this._CanAsync = false;

                            break;
                    }
                    break;
            }
        }

        public override bool Searchable => false;
        public override bool CanAsync => this._CanAsync;

        public object ObjectResult { get; private set; }

        public override void Parse()
        { }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            if (string.IsNullOrEmpty(this._RawValue))
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                this.ObjectResult = null;

                return;
            }

            switch (this._RawValue)
            {
                case "DomainContents":
                    this.RenderDomainContents();

                    break;
                case "PageRenderDuration":
                    this.RenderPageRenderDuration();

                    break;
                default:
                    switch (this._RawValue[0])
                    {
                        case '^':
                            // QueryString Value
                            this.RenderQueryString();

                            break;
                        case '~':
                            // Form Post Value
                            this.RenderFormPost();

                            break;
                        case '-':
                            // Session Value
                            this.RenderSessionItem();

                            break;
                        case '+':
                            // Cookies Value
                            this.RenderCookieItem();

                            break;
                        case '=':
                            // Value which following after '='
                            this.RenderStaticString();

                            break;
                        case '#':
                            // DataTable Field
                            this.RenderDataItem();

                            break;
                        case '*':
                            // Search in All orderby : [InData, DataField, Session, Form Post, QueryString, Cookie] (DOES NOT SUPPORT FILE POSTS)
                            string searchArgKey = this._RawValue.Substring(1);
                            object searchArgValue = null;

                            // Search InDatas
                            searchArgValue = this.Arguments[searchArgKey];

                            // Search In VariablePool
                            if (searchArgValue == null)
                                searchArgValue = Helpers.VariablePool.Get(searchArgKey);

                            // Search In Session
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Session[searchArgKey];

                            // Cookie
                            if (searchArgValue == null &&
                                (Helpers.Context.Request.Header.Cookie[searchArgKey] != null))
                            {
                                searchArgValue = Helpers.Context.Request.Header.Cookie[searchArgKey].Value;
                            }

                            // Search In Form Post First File then Value
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Request.Body.File[searchArgKey];
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Request.Body.Form[searchArgKey];

                            // Search QueryString
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Request.QueryString[searchArgKey];

                            if (searchArgValue != null)
                                this.Deliver(RenderStatus.Rendered, searchArgValue.ToString());
                            else
                                this.Deliver(RenderStatus.Rendered, string.Empty);
                            this.ObjectResult = searchArgValue;

                            break;
                        case '@':
                            // Search in Values Set for Current Request Session
                            this.RenderObjectItem();

                            break;
                        default:
                            this.RenderVariablePoolItem();

                            break;
                    }
                    break;
            }
        }

        private void RenderDomainContents()
        {
            Basics.Domain.IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            this.Deliver(RenderStatus.Rendered, instance.ContentsVirtualPath);
            this.ObjectResult = (object)instance.ContentsVirtualPath;
        }

        private void RenderPageRenderDuration()
        {
            string pointer = "<!--_sys_PAGERENDERDURATION-->";

            this.Deliver(RenderStatus.Rendered, pointer);
            this.ObjectResult = (object)pointer;
        }

        private void RenderQueryString()
        {
            string queryItemKey = this._RawValue.Substring(1);
            string queryItemValue = 
                Helpers.Context.Request.QueryString[queryItemKey];

            if (!string.IsNullOrEmpty(queryItemValue))
            {
                switch (Configurations.Xeora.Application.RequestTagFilter.Direction)
                {
                    case Basics.Configuration.RequestTagFilteringTypes.OnlyQuery:
                    case Basics.Configuration.RequestTagFilteringTypes.Both:
                        if (Array.IndexOf(Configurations.Xeora.Application.RequestTagFilter.Exceptions, queryItemKey) == -1)
                            queryItemValue = this.CleanHTMLTags(queryItemValue, Configurations.Xeora.Application.RequestTagFilter.Items);

                        break;
                }
            }

            this.Deliver(RenderStatus.Rendered, queryItemValue);
            this.ObjectResult = (object)queryItemValue;
        }

        private void RenderFormPost()
        {
            string formItemKey = this._RawValue.Substring(1);

            // File Post is not supporting XML Http Requests
            string[] keys = Helpers.Context.Request.Body.File.Keys;
            List<Basics.Context.IHttpRequestFileInfo> requestFileObjects =
                new List<Basics.Context.IHttpRequestFileInfo>();

            for (int kC = 0; kC < keys.Length; kC++)
            {
                if (string.Compare(keys[kC], formItemKey, true) != 0)
                    continue;

                requestFileObjects.Add(Helpers.Context.Request.Body.File[keys[kC]]);
            }
            // !--

            if (requestFileObjects.Count > 0)
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                if (requestFileObjects.Count == 1)
                    this.ObjectResult = requestFileObjects[0];
                else
                    this.ObjectResult = requestFileObjects.ToArray();

                return;
            }

            string formItemValue = 
                Helpers.Context.Request.Body.Form[formItemKey];

            if (!string.IsNullOrEmpty(formItemValue))
            {
                switch (Configurations.Xeora.Application.RequestTagFilter.Direction)
                {
                    case Basics.Configuration.RequestTagFilteringTypes.OnlyForm:
                    case Basics.Configuration.RequestTagFilteringTypes.Both:
                        if (Array.IndexOf(Configurations.Xeora.Application.RequestTagFilter.Exceptions, formItemKey) == -1)
                            formItemValue = this.CleanHTMLTags(formItemValue, Configurations.Xeora.Application.RequestTagFilter.Items);

                        break;
                }
            }

            this.Deliver(RenderStatus.Rendered, formItemValue);
            this.ObjectResult = formItemValue;
        }

        private void RenderSessionItem()
        {
            string sessionItemKey = this._RawValue.Substring(1);
            object sessionItemValue = 
                Helpers.Context.Session[sessionItemKey];

            if (sessionItemValue == null)
                this.Deliver(RenderStatus.Rendered, string.Empty);
            else
                this.Deliver(RenderStatus.Rendered, sessionItemValue.ToString());
            this.ObjectResult = sessionItemValue;
        }

        private void RenderCookieItem()
        {
            string cookieItemKey = this._RawValue.Substring(1);
            Basics.Context.IHttpCookieInfo cookieItem = 
                Helpers.Context.Request.Header.Cookie[cookieItemKey];

            if (cookieItem == null)
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                this.ObjectResult = null;

                return;
            }

            this.Deliver(RenderStatus.Rendered, cookieItem.Value);
            this.ObjectResult = cookieItem.Value;
        }

        private void RenderStaticString()
        {
            string stringValue = 
                this._RawValue.Substring(1);

            this.Deliver(RenderStatus.Rendered, stringValue);
            this.ObjectResult = stringValue;
        }

        private void RenderDataItem()
        {
            string searchVariableKey = this._RawValue;

            IDirective searchDirective = this;
            this.LocateLeveledContentInfo(ref searchVariableKey, ref searchDirective);

            if (searchDirective == null)
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                this.ObjectResult = null;

                return;
            }

            object argItem =
                searchDirective.Arguments[searchVariableKey];

            if (argItem != null &&
                !object.ReferenceEquals(argItem.GetType(), typeof(DBNull)))
            {
                this.Deliver(RenderStatus.Rendered, argItem.ToString());
                this.ObjectResult = argItem;

                return;
            }

            this.Deliver(RenderStatus.Rendered, string.Empty);
            this.ObjectResult = null;
        }

        private void RenderVariablePoolItem()
        {
            object poolValue = Helpers.VariablePool.Get(this._RawValue);

            if (poolValue != null)
                this.Deliver(RenderStatus.Rendered, poolValue.ToString());
            else
                this.Deliver(RenderStatus.Rendered, string.Empty);
            this.ObjectResult = poolValue;
        }

        private void RenderObjectItem()
        {
            string objectPath = this._RawValue.Substring(1);
            string[] objectPaths = objectPath.Split('.');

            if (objectPaths.Length < 2)
                throw new Exception.GrammerException();

            string objectItemKey = objectPaths[0];
            object objectItem = null;

            switch (objectItemKey[0])
            {
                case '-':
                    objectItem = Helpers.Context.Session[objectItemKey.Substring(1)];

                    break;
                case '#':
                    IDirective searchDirective = this;
                    this.LocateLeveledContentInfo(ref objectItemKey, ref searchDirective);

                    object argItem = searchDirective?.Arguments[objectItemKey];

                    if (argItem != null)
                        objectItem = argItem;

                    break;
                default:
                    objectItem = Helpers.VariablePool.Get(objectItemKey);

                    if (objectItem == null || objectItem is DataListOutputInfo)
                    {
                        IDirective parentDirective = null;
                        if (objectItem != null)
                            this.Mother.Pool.GetInto(((DataListOutputInfo)objectItem).UniqueID, out parentDirective);

                        if (parentDirective == null)
                        {
                            this.Mother.Scheduler.Register(objectItemKey, this.UniqueID);

                            return;
                        }
                    }

                    break;
            }

            if (objectItem == null)
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                this.ObjectResult = null;

                return;
            }

            object objectValue;
            try
            {
                for (int pC = 1; pC < objectPaths.Length; pC++)
                {
                    if (objectItem == null)
                        break;

                    objectItem = objectItem.GetType().InvokeMember(objectPaths[pC], BindingFlags.GetProperty, null, objectItem, null);
                }

                objectValue = objectItem;
            }
            catch (System.Exception)
            {
                objectValue = null;
            }

            if (objectValue != null)
                this.Deliver(RenderStatus.Rendered, objectValue.ToString());
            else
                this.Deliver(RenderStatus.Rendered, string.Empty);
            this.ObjectResult = objectValue;
        }

        private void LocateLeveledContentInfo(ref string searchItemKey, ref IDirective directive)
        {
            searchItemKey = searchItemKey.Substring(1);

            while (searchItemKey.IndexOf("#", StringComparison.InvariantCulture) == 0)
            {
                directive = directive.Parent;
                searchItemKey = searchItemKey.Substring(1);
            } 
        }

        private string CleanHTMLTags(string content, string[] cleaningTags)
        {
            Regex regExSearch;

            if (string.IsNullOrEmpty(content) || cleaningTags == null || cleaningTags.Length == 0)
                return content;

            int searchType, lastSearchIndex;
            StringBuilder modifiedContent;
            MatchCollection regExMatches;

            foreach (string cleaningTag in cleaningTags)
            {
                if (cleaningTag.IndexOf('>') == 0)
                {
                    regExSearch = new Regex(string.Format("<{0}(\\s+[^>]*)*>", cleaningTag.Substring(1)));
                    searchType = 1;
                }
                else
                {
                    regExSearch = new Regex(string.Format("<{0}(\\s+[^>]*)*(/)?>", cleaningTag));
                    searchType = 0;
                }

                regExMatches = regExSearch.Matches(content);

                modifiedContent = new StringBuilder();
                lastSearchIndex = 0;

                foreach (Match regMatch in regExMatches)
                {
                    modifiedContent.Append(content.Substring(lastSearchIndex, regMatch.Index - lastSearchIndex));

                    switch (searchType)
                    {
                        case 1:
                            Regex TailRegExSearch = new Regex(string.Format("</{0}>", cleaningTag.Substring(1)));
                            Match TailRegMatch = TailRegExSearch.Match(content, lastSearchIndex);

                            if (TailRegMatch.Success)
                                lastSearchIndex = TailRegMatch.Index + TailRegMatch.Length;
                            else
                                lastSearchIndex = regMatch.Index + regMatch.Length;

                            break;
                        default:
                            lastSearchIndex = regMatch.Index + regMatch.Length;

                            break;
                    }
                }
                modifiedContent.Append(content.Substring(lastSearchIndex));

                content = modifiedContent.ToString();
            }

            return content;
        }
    }
}