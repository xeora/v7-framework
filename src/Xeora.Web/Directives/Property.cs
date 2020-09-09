using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives
{
    public class Property
    {
        private readonly IDirective _Directive;
        private readonly ArgumentCollection _Arguments;
        private readonly string _RawValue;

        private Property(IDirective directive, string rawValue)
        {
            this._Directive = directive;
            this._Arguments = directive?.Arguments;
            this._RawValue = rawValue;
        }

        public static Tuple<bool, object> Render(IDirective directive, string rawValue)
        {
            Property objectProperty = 
                new Property(directive, rawValue);
            return objectProperty.Render();
        }
        
        private Tuple<bool, object> Render()
        {
            if (string.IsNullOrEmpty(this._RawValue)) return new Tuple<bool, object>(true, string.Empty);

            switch (this._RawValue)
            {
                case "DomainContents":
                    return this.RenderDomainContents();
                case "PageRenderDuration":
                    return Property.RenderPageRenderDuration();
                default:
                    switch (this._RawValue[0])
                    {
                        case '^':
                            // QueryString Value
                            return this.RenderQueryString();
                        case '~':
                            // Form Post Value
                            return this.RenderFormPost();
                        case '-':
                            // Session Value
                            return this.RenderSessionItem();
                        case '&':
                            // Application Value
                            return this.RenderApplicationItem();
                        case '+':
                            // Cookies Value
                            return this.RenderCookieItem();
                        case '=':
                            // Value which following after '='
                            return this.RenderStaticString();
                        case '#':
                            // DataTable Field
                            return this.RenderDataItem();
                        case '*':
                            // Search in All orderby : [InData, DataField, Session, Application, Form Post, QueryString, Cookie] (DOES NOT SUPPORT FILE POSTS)
                            string searchArgKey = 
                                this._RawValue.Substring(1);

                            // Search InDatas
                            object searchArgValue = this._Arguments?[searchArgKey];

                            // Search In VariablePool
                            if (searchArgValue == null)
                                searchArgValue = Helpers.VariablePool.Get(searchArgKey);

                            // Search In Session
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Session[searchArgKey];

                            // Search In Application
                            if (searchArgValue == null)
                                searchArgValue = Helpers.Context.Application[searchArgKey];
                            
                            // Cookie
                            if (searchArgValue == null &&
                                Helpers.Context.Request.Header.Cookie[searchArgKey] != null)
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

                            return new Tuple<bool, object>(true, searchArgValue);
                        case '@':
                            // Search in Values Set for Current Request Session
                            return this.RenderObjectItem();
                        default:
                            return this.RenderVariablePoolItem();
                    }
            }
        }

        private Tuple<bool, object> RenderDomainContents()
        {
            if (this._Directive == null) return new Tuple<bool, object>(true, null);
            
            this._Directive.Mother.RequestInstance(out Basics.Domain.IDomain instance);
            return new Tuple<bool, object>(true, instance.ContentsVirtualPath);
        }

        private static Tuple<bool, object> RenderPageRenderDuration() => 
            new Tuple<bool, object>(true, "<!--_sys_PAGERENDERDURATION-->");

        private Tuple<bool, object> RenderQueryString()
        {
            string queryItemKey = this._RawValue.Substring(1);
            string queryItemValue = 
                Helpers.Context.Request.QueryString[queryItemKey];

            if (string.IsNullOrEmpty(queryItemValue)) 
                return new Tuple<bool, object>(true, string.Empty);
            
            switch (Configurations.Xeora.Application.RequestTagFilter.Direction)
            {
                case Basics.Configuration.RequestTagFilteringTypes.OnlyQuery:
                case Basics.Configuration.RequestTagFilteringTypes.Both:
                    if (Array.IndexOf(Configurations.Xeora.Application.RequestTagFilter.Exceptions, queryItemKey) == -1)
                        queryItemValue = Property.CleanHtmlTags(queryItemValue, Configurations.Xeora.Application.RequestTagFilter.Items);

                    break;
            }
            return new Tuple<bool, object>(true, queryItemValue);
        }

        private Tuple<bool, object> RenderFormPost()
        {
            string formItemKey = this._RawValue.Substring(1);

            // File Post is not supporting XML Http Requests
            string[] keys = Helpers.Context.Request.Body.File.Keys;
            List<Basics.Context.Request.IHttpRequestFileInfo> requestFileObjects =
                new List<Basics.Context.Request.IHttpRequestFileInfo>();

            foreach (string key in keys)
            {
                if (string.Compare(key, formItemKey, StringComparison.OrdinalIgnoreCase) != 0)
                    continue;

                requestFileObjects.Add(Helpers.Context.Request.Body.File[key]);
            }
            // !--

            if (requestFileObjects.Count > 0)
                return requestFileObjects.Count == 1 ? 
                        new Tuple<bool, object>(true, requestFileObjects[0]) : 
                        new Tuple<bool, object>(true, requestFileObjects.ToArray());

            string formItemValue = 
                Helpers.Context.Request.Body.Form[formItemKey];

            if (string.IsNullOrEmpty(formItemValue)) 
                return new Tuple<bool, object>(true, string.Empty);
            
            switch (Configurations.Xeora.Application.RequestTagFilter.Direction)
            {
                case Basics.Configuration.RequestTagFilteringTypes.OnlyForm:
                case Basics.Configuration.RequestTagFilteringTypes.Both:
                    if (Array.IndexOf(Configurations.Xeora.Application.RequestTagFilter.Exceptions, formItemKey) == -1)
                        formItemValue = Property.CleanHtmlTags(formItemValue, Configurations.Xeora.Application.RequestTagFilter.Items);

                    break;
            }
            return new Tuple<bool, object>(true, formItemValue);
        }

        private Tuple<bool, object> RenderSessionItem()
        {
            string sessionItemKey = 
                this._RawValue.Substring(1);
            return new Tuple<bool, object>(true, Helpers.Context.Session[sessionItemKey]);
        }
        
        private Tuple<bool, object> RenderApplicationItem()
        {
            string applicationItemKey = 
                this._RawValue.Substring(1);
            return new Tuple<bool, object>(true, Helpers.Context.Application[applicationItemKey]);
        }

        private Tuple<bool, object> RenderCookieItem()
        {
            string cookieItemKey = 
                this._RawValue.Substring(1);
            Basics.Context.IHttpCookieInfo cookieItem = 
                Helpers.Context.Request.Header.Cookie[cookieItemKey];

            return new Tuple<bool, object>(true, cookieItem == null ? string.Empty : cookieItem.Value);
        }

        private Tuple<bool, object> RenderStaticString() => 
            new Tuple<bool, object>(true, this._RawValue.Substring(1));

        private Tuple<bool, object> RenderDataItem()
        {
            if (this._Directive == null) return new Tuple<bool, object>(true, null);
            
            string searchVariableKey = this._RawValue;

            IDirective searchDirective = this._Directive;
            Property.LocateLeveledContentInfo(ref searchVariableKey, ref searchDirective);

            object argItem =
                searchDirective?.Arguments[searchVariableKey];

            if (argItem == null || ReferenceEquals(argItem.GetType(), typeof(DBNull))) 
                return new Tuple<bool, object>(true, null);

            return new Tuple<bool, object>(true, argItem);
        }

        private Tuple<bool, object> RenderVariablePoolItem() => 
            new Tuple<bool, object>(true, Helpers.VariablePool.Get(this._RawValue));

        private Tuple<bool, object> RenderObjectItem()
        {
            string objectPath = 
                this._RawValue.Substring(1);

            string[] objectPaths;
            if (objectPath.StartsWith('.'))
            {
                objectPaths = objectPath.Substring(1).Split('.');
                objectPaths[0] = $".{objectPaths[0]}";
            }
            else
                objectPaths = objectPath.Split('.');

            if (objectPaths.Length < 2)
                throw new Exceptions.GrammarException();

            string objectItemKey = objectPaths[0];
            object objectItem;

            switch (objectItemKey[0])
            {
                case '-':
                    objectItem = Helpers.Context.Session[objectItemKey.Substring(1)];

                    break;
                case '&':
                    objectItem = Helpers.Context.Application[objectItemKey.Substring(1)];

                    break;
                case '#':
                    IDirective searchDirective = this._Directive;
                    Property.LocateLeveledContentInfo(ref objectItemKey, ref searchDirective);

                    objectItem = 
                        searchDirective?.Arguments[objectItemKey];
                    if (objectItem is DataListOutputInfo)
                        objectItem = null;

                    break;
                case '.':
                    objectItem = this._Directive.Arguments[objectItemKey.Substring(1)];

                    break;
                default:
                    objectItem = Helpers.VariablePool.Get(objectItemKey);

                    if (objectItem == null) break;

                    if (objectItem is DataListOutputInfo outputInfo)
                    {
                        if (this._Directive == null) return new Tuple<bool, object>(true, null);
                        
                        string uniqueId =
                            outputInfo.UniqueId;
                        this._Directive.Mother.Pool.GetByUniqueId(uniqueId, out IDirective directive);

                        if (directive.Status != RenderStatus.Rendered)
                        {
                            directive.Scheduler.Register(this._Directive.UniqueId);
                            return new Tuple<bool, object>(false, null);
                        }
                    }

                    break;
            }

            if (objectItem == null) return new Tuple<bool, object>(true, null);
            
            Monitor.Enter(this._Directive.Mother.PropertyLock);
            try
            {
                for (int i = 1; i < objectPaths.Length; i++)
                {
                    if (objectItem == null)
                        break;

                    string invokeMember =
                        objectPaths[i];
                    BindingFlags invokeAttribute =
                        BindingFlags.GetProperty;
                    object[] invokeParameters = null;

                    Type objectType = 
                        objectItem.GetType();
                    
                    if (objectItem is IDictionary || objectItem is IDictionary<object, object>)
                    {
                        invokeMember = "Item";
                        invokeAttribute = BindingFlags.GetProperty;
                        invokeParameters = new object[] {objectPaths[i]};
                    }
                    else if (objectType.IsArray && long.TryParse(invokeMember, out long itemIndex))
                    {
                        invokeMember = "GetValue";
                        invokeAttribute = BindingFlags.InvokeMethod;
                        invokeParameters = new object[] {itemIndex};
                    }
                    else
                    {
                        MemberInfo[] memberInfos =
                            objectType.GetMember("Item");

                        if (memberInfos.Length > 0)
                        {
                            invokeMember = "Item";
                            invokeAttribute = BindingFlags.GetProperty;
                            invokeParameters = new object[] {objectPaths[i]};
                        }
                    }
                    
                    objectItem =
                        objectItem.GetType().InvokeMember(
                            invokeMember,
                            invokeAttribute,
                            null,
                            objectItem,
                            invokeParameters);
                }

                return new Tuple<bool, object>(true, objectItem);
            }
            catch (Exception)
            {
                return new Tuple<bool, object>(true, null);
            }
            finally
            {
                Monitor.Exit(this._Directive.Mother.PropertyLock);
            }
        }

        private static void LocateLeveledContentInfo(ref string searchItemKey, ref IDirective directive)
        {
            do
            {
                if (directive == null) return;
                if (searchItemKey.IndexOf("#", StringComparison.InvariantCulture) != 0) return;
                
                searchItemKey = 
                    searchItemKey.Substring(1);
                do
                {
                    directive = directive.Parent;
                } while (directive != null && !directive.CanHoldVariable);
            } while (true);
        }

        private static string CleanHtmlTags(string content, IReadOnlyCollection<string> cleaningTags)
        {
            if (string.IsNullOrEmpty(content) || cleaningTags == null || cleaningTags.Count == 0)
                return content;

            foreach (string cleaningTag in cleaningTags)
            {
                Regex regExSearch;
                int searchType;
                
                if (cleaningTag.IndexOf('>') == 0)
                {
                    regExSearch = new Regex($"<{cleaningTag.Substring(1)}(\\s+[^>]*)*>");
                    searchType = 1;
                }
                else
                {
                    regExSearch = new Regex($"<{cleaningTag}(\\s+[^>]*)*(/)?>");
                    searchType = 0;
                }

                MatchCollection regExMatches = regExSearch.Matches(content);
                StringBuilder modifiedContent = new StringBuilder();
                int lastSearchIndex = 0;

                foreach (Match regMatch in regExMatches)
                {
                    modifiedContent.Append(content.Substring(lastSearchIndex, regMatch.Index - lastSearchIndex));

                    switch (searchType)
                    {
                        case 1:
                            Regex tailRegExSearch = new Regex($"</{cleaningTag.Substring(1)}>");
                            Match tailRegMatch = tailRegExSearch.Match(content, lastSearchIndex);

                            if (tailRegMatch.Success)
                            {
                                lastSearchIndex = tailRegMatch.Index + tailRegMatch.Length;
                                break;
                            }
                            
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