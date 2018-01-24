using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class Property : Controller, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;

        public Property(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, ControllerTypes.Property, contentArguments)
        {
            this.ObjectResult = null;
        }

        public object ObjectResult { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            if (string.IsNullOrEmpty(this.Value))
            {
                if (string.Compare(this.RawValue, this.Value) != 0)
                {
                    this.RenderedValue = "$";
                    this.ObjectResult = (object)"$";

                    return;
                }

                this.RenderedValue = string.Empty;
                this.ObjectResult = null;

                return;
            }

            switch (this.Value)
            {
                case "DomainContents":
                    this.RenderDomainContents();

                    break;
                case "PageRenderDuration":
                    this.RenderPageRenderDuration();

                    break;
                default:
                    switch (this.Value[0])
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
                            string searchArgKey = this.Value.Substring(1);
                            object searchArgValue = null;

                            // Search InDatas
                            searchArgValue = this.ContentArguments[searchArgKey];

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
                                this.RenderedValue = searchArgValue.ToString();
                            else
                                this.RenderedValue = string.Empty;
                            this.ObjectResult = searchArgValue;

                            break;
                        case '@':
                            // Search in Values Set for Current Request Session
                            this.RenderObjectItem(requesterUniqueID);

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
            InstanceRequested?.Invoke(ref instance);

            this.RenderedValue = instance.ContentsVirtualPath;
            this.ObjectResult = (object)this.RenderedValue;
        }

        private void RenderPageRenderDuration()
        {
            this.RenderedValue = "<!--_sys_PAGERENDERDURATION-->";
            this.ObjectResult = (object)"<!--_sys_PAGERENDERDURATION-->";
        }

        private void RenderQueryString()
        {
            string queryItemKey = this.Value.Substring(1);
            string queryItemValue = Helpers.Context.Request.QueryString[queryItemKey];

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

            this.RenderedValue = queryItemValue;
            this.ObjectResult = (object)queryItemValue;
        }

        private void RenderFormPost()
        {
            string formItemKey = this.Value.Substring(1);

            // File Post is not supporting XML Http Requests
            string[] keys = Helpers.Context.Request.Body.File.Keys;
            List<Basics.Context.IHttpRequestFileInfo> requestFileObjects =
                new List<Basics.Context.IHttpRequestFileInfo>();

            for (int kC = 0; kC < keys.Length; kC++)
            {
                if (string.Compare(keys[kC], formItemKey, true) == 0)
                    requestFileObjects.Add(Helpers.Context.Request.Body.File[keys[kC]]);
            }
            // !--

            if (requestFileObjects.Count > 0)
            {
                this.RenderedValue = string.Empty;
                if (requestFileObjects.Count == 1)
                    this.ObjectResult = requestFileObjects[0];
                else
                    this.ObjectResult = requestFileObjects.ToArray();

                return;
            }

            string formItemValue = Helpers.Context.Request.Body.Form[formItemKey];

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

            this.RenderedValue = formItemValue;
            this.ObjectResult = formItemValue;
        }

        private void RenderSessionItem()
        {
            string sessionItemKey = this.Value.Substring(1);
            object sessionItemValue = Helpers.Context.Session[sessionItemKey];

            if (sessionItemValue == null)
                this.RenderedValue = string.Empty;
            else
                this.RenderedValue = sessionItemValue.ToString();
            this.ObjectResult = sessionItemValue;
        }

        private void RenderCookieItem()
        {
            string cookieItemKey = this.Value.Substring(1);
            Basics.Context.IHttpCookieInfo cookieItem = Helpers.Context.Request.Header.Cookie[cookieItemKey];

            if (cookieItem == null)
            {
                this.RenderedValue = string.Empty;
                this.ObjectResult = null;

                return;
            }

            this.RenderedValue = cookieItem.Value;
            this.ObjectResult = cookieItem.Value;
        }

        private void RenderStaticString()
        {
            string stringValue = this.Value.Substring(1);

            this.RenderedValue = stringValue;
            this.ObjectResult = stringValue;
        }

        private void RenderDataItem()
        {
            string searchVariableKey = this.Value;

            IController searchController = this;
            this.LocateLeveledContentInfo(ref searchVariableKey, ref searchController);

            if (searchController == null)
            {
                this.RenderedValue = string.Empty;
                this.ObjectResult = null;

                return;
            }

            object argItem = searchController.ContentArguments[searchVariableKey];

            if (argItem != null &&
                !object.ReferenceEquals(argItem.GetType(), typeof(DBNull)))
            {
                this.RenderedValue = argItem.ToString();
                this.ObjectResult = argItem;

                return;
            }

            this.RenderedValue = string.Empty;
            this.ObjectResult = null;
        }

        private void RenderVariablePoolItem()
        {
            object poolValue = Helpers.VariablePool.Get(this.Value);

            if (poolValue != null)
                this.RenderedValue = poolValue.ToString();
            else
                this.RenderedValue = string.Empty;
            this.ObjectResult = poolValue;
        }

        private void RenderObjectItem(string parentUniqueID)
        {
            string objectPath = this.Value.Substring(1);
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
                    IController searchController = this;
                    this.LocateLeveledContentInfo(ref objectItemKey, ref searchController);

                    if (searchController != null)
                    {
                        object argItem = searchController.ContentArguments[objectItemKey];

                        if (argItem != null)
                            objectItem = argItem;
                    }

                    break;
                default:
                    objectItem = Helpers.VariablePool.Get(objectItemKey);

                    if (objectItem == null || objectItem is DataListOutputInfo)
                    {
                        IController parentController = null;
                        if (objectItem != null)
                            this.Mother.Pool.GetInto(((DataListOutputInfo)objectItem).UniqueID, out parentController);

                        if (parentController == null)
                        {
                            this.Mother.Scheduler.Register(objectItemKey, this.UniqueID);

                            return;
                        }
                    }

                    break;
            }

            if (objectItem == null)
            {
                this.RenderedValue = string.Empty;
                this.ObjectResult = null;

                return;
            }

            object objectValue = null;
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
                this.RenderedValue = objectValue.ToString();
            else
                this.RenderedValue = string.Empty;
            this.ObjectResult = objectValue;
        }

        private void LocateLeveledContentInfo(ref string searchItemKey, ref IController controller)
        {
            bool outsideOfBlock = (searchItemKey.LastIndexOf('#') > 0);
            do
            {
                if (searchItemKey.IndexOf("#") == 0)
                {
                    do
                    {
                        controller = controller.Parent;

                        // Only Controls that have own contents such as Datalist, VariableBlock and MessageBlock!
                        // however, Datalist can have multiple content that's why we should check it this content
                        // parent is Datalist or not
                    } while (controller != null && 
                        !(controller.Parent is Control.DataList || 
                        controller is Control.VariableBlock || 
                        controller is MessageBlock));
                }
                else
                {
                    if (controller is Control.VariableBlock &&
                        ((Control.VariableBlock)controller).Leveling.Level > 0 &&
                        !((Control.VariableBlock)controller).Leveling.ExecutionOnly &&
                        outsideOfBlock
                    )
                    {
                        outsideOfBlock = false;
                        searchItemKey = searchItemKey.PadLeft(((Control.VariableBlock)controller).Leveling.Level + searchItemKey.Length, '#');
                    }
                    else
                        break;
                }

                searchItemKey = searchItemKey.Substring(1);
            } while (controller != null);
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