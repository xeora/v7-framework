using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Xeora.Web.Controller;
using Xeora.Web.Controller.Directive;
using Xeora.Web.Controller.Directive.Control;
using Xeora.Web.Manager;

namespace Xeora.Web.Site
{
    public class Domain : Basics.IDomain
    {
        private Renderer _Renderer = null;
        private Deployment.DomainDeployment _Deployment = null;

        private LanguageHolder _LanguageHolder;
        private XPathNavigator _ControlsXPathNavigator;

        public Domain(string[] domainIDAccessTree) :
            this(domainIDAccessTree, null)
        { }

        public Domain(string[] domainIDAccessTree, string languageID) =>
            this.BuildDomain(domainIDAccessTree, languageID);

        private void BuildDomain(string[] domainIDAccessTree, string languageID)
        {
            if (this._xPathStream != null)
            {
                this._xPathStream.Close();
                this._ControlsXPathNavigator = null;
            }

            if (domainIDAccessTree == null)
                domainIDAccessTree = Basics.Configurations.Xeora.Application.Main.DefaultDomain;

            try
            {
                this._Deployment = Deployment.InstanceFactory.Current.GetOrCreate(domainIDAccessTree, languageID);
            }
            catch (Exception.DomainNotExistsException)
            {
                // Try with the default one if requested one is not the default one
                if (string.Compare(
                    string.Join("\\", domainIDAccessTree),
                    string.Join("\\", Basics.Configurations.Xeora.Application.Main.DefaultDomain)) != 0)
                    this._Deployment =
                        Deployment.InstanceFactory.Current.GetOrCreate(
                            Basics.Configurations.Xeora.Application.Main.DefaultDomain, languageID);
                else
                    throw;
            }
            catch (System.Exception)
            {
                throw;
            }

            this._LanguageHolder = new LanguageHolder(this, this._Deployment.Language);

            if (domainIDAccessTree.Length > 1)
            {
                string[] ParentDomainIDAccessTree = new string[domainIDAccessTree.Length - 1];
                Array.Copy(domainIDAccessTree, 0, ParentDomainIDAccessTree, 0, ParentDomainIDAccessTree.Length);

                this.Parent = new Domain(ParentDomainIDAccessTree, this._Deployment.LanguageID);
            }

            if (this._Renderer == null)
                this._Renderer = new Renderer();

            this._Renderer.Inject(this);
        }

        public string[] IDAccessTree => this._Deployment.DomainIDAccessTree;
        public Basics.DomainInfo.DeploymentTypes DeploymentType => this._Deployment.DeploymentType;

        public Basics.IDomain Parent { get; private set; }
        public Basics.DomainInfo.DomainInfoCollection Children => this._Deployment.Children;

        public string ContentsVirtualPath =>
            string.Format(
                "{0}{1}_{2}",
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                string.Join<string>("-", this._Deployment.DomainIDAccessTree),
                this._Deployment.Language.ID
            );

        public Basics.ISettings Settings => this._Deployment.Settings;
        public Basics.ILanguage Language => this._LanguageHolder;
        public Basics.IxService xService => this._Deployment.xService;

        public void ProvideFileStream(string requestedFilePath, out Stream outputStream) =>
            this._Deployment.ProvideFileStream(requestedFilePath, out outputStream);

        public void PushLanguageChange(string languageID) =>
            this.BuildDomain(this._Deployment.DomainIDAccessTree, languageID);

        public void ClearCache()
        {
            AssemblyCore.ClearCache();

            this._Renderer.ClearCache();
            PartialCache.ClearCache(this.IDAccessTree);

            Deployment.InstanceFactory.Current.Reset();
        }

        public string Render(Basics.ServicePathInfo servicePathInfo, Basics.ControlResult.Message messageResult, string updateBlockControlID = null) =>
            this._Renderer.Start(servicePathInfo, messageResult, updateBlockControlID);

        public string Render(string xeoraContent, Basics.ControlResult.Message messageResult, string updateBlockControlID = null) =>
            this._Renderer.Start(xeoraContent, messageResult, updateBlockControlID);

        private StringReader _xPathStream = null;
        private XPathNavigator ControlsXPathNavigator
        {
            get
            {
                if (this._ControlsXPathNavigator != null)
                    return this._ControlsXPathNavigator;

                XPathDocument xPathDoc = null;
                string ControlMapContent =
                    this._Deployment.ProvideControlsContent();

                if (this._xPathStream != null)
                {
                    this._xPathStream.Close();
                    GC.SuppressFinalize(this._xPathStream);
                }

                this._xPathStream = new StringReader(ControlMapContent);
                xPathDoc = new XPathDocument(this._xPathStream);

                this._ControlsXPathNavigator = xPathDoc.CreateNavigator();

                return this._ControlsXPathNavigator;
            }
        }

        public void Dispose()
        {
            if (this._xPathStream != null)
            {
                this._xPathStream.Close();
                GC.SuppressFinalize(this._xPathStream);
            }
            GC.SuppressFinalize(this);
        }

        private class LanguageHolder : Basics.ILanguage
        {
            private Basics.IDomain _Owner;
            private Basics.ILanguage _Language;

            public LanguageHolder(Basics.IDomain owner, Basics.ILanguage language)
            {
                this._Owner = owner;
                this._Language = language;
            }

            public string ID => this._Language.ID;
            public string Name => this._Language.Name;
            public Basics.DomainInfo.LanguageInfo Info => this._Language.Info;

            public string Get(string translationID)
            {
                try
                {
                    return this._Language.Get(translationID);
                }
                catch (Exception.TranslationNotFoundException)
                {
                    if (this._Owner.Parent != null)
                        return this._Owner.Parent.Language.Get(translationID);
                }

                return null;
            }

            public void Dispose()
            { /* Dispose will be handled by InstanceFactory. No need to handle here! */ }
        }

        private class Renderer
        {
            private Basics.IDomain _Instance = null;

            public void Inject(Basics.IDomain instance) =>
                this._Instance = instance;

            public string Start(Basics.ServicePathInfo servicePathInfo, Basics.ControlResult.Message messageResult, string updateBlockControlID = null) =>
                this.Start(string.Format("$T:{0}$", servicePathInfo.FullPath), messageResult, updateBlockControlID);

            public string Start(string xeoraContent, Basics.ControlResult.Message messageResult, string updateBlockControlID = null)
            {
                if (this._Instance == null)
                    throw new System.Exception("Injection required!");

                Mother ExternalController =
                    new Mother(0, xeoraContent, messageResult, updateBlockControlID);
                ExternalController.ParseRequested += this.OnParseRequest;
                ExternalController.Setup();

                ExternalController.Render(null);

                return ExternalController.RenderedValue;
            }

            private void OnParseRequest(string rawValue, ref ControllerCollection childrenContainer, Global.ArgumentInfoCollection contentArguments)
            {
                MatchCollection mainPatternMatches = RegularExpression.Current.MainCapturePattern.Matches(rawValue);

                if (mainPatternMatches.Count == 0)
                    childrenContainer.Add(new Renderless(0, rawValue, contentArguments));
                else
                {
                    int lastIndex = 0;

                    Match mainSearchMatch;
                    // For Opening Brackets
                    Match bracketOpenExamMatch;
                    string directiveType, directiveID;
                    // For Separator Brackets
                    Match bracketSeparatorExamMatch;
                    // For Closing Brackets
                    Match bracketCloseExamMatch;

                    IEnumerator remEnum = mainPatternMatches.GetEnumerator();

                    while (remEnum.MoveNext())
                    {
                        mainSearchMatch = (Match)remEnum.Current;

                        // Check till this match any renderless content exists
                        if (mainSearchMatch.Index > lastIndex)
                        {
                            childrenContainer.Add(
                                new Renderless(
                                    lastIndex,
                                    rawValue.Substring(lastIndex, mainSearchMatch.Index - lastIndex),
                                    contentArguments
                                )
                            );
                            lastIndex = mainSearchMatch.Index;
                        }

                        // Exam For Bracketed Regex Result
                        bracketOpenExamMatch = RegularExpression.Current.BracketedControllerOpenPattern.Match(mainSearchMatch.Value);

                        if (bracketOpenExamMatch.Success)
                        {
                            directiveType = bracketOpenExamMatch.Result("${DirectiveType}");
                            directiveID = bracketOpenExamMatch.Result("${ItemID}");

                            if (directiveID != null)
                            {
                                int innerMatch = 0;
                                List<int> separatorIndexes = new List<int>();

                                while (remEnum.MoveNext())
                                {
                                    Match mainSearchMatchExam = (Match)remEnum.Current;

                                    // Exam For Opening Bracketed Regex Result
                                    bracketOpenExamMatch =
                                        RegularExpression.Current.BracketedControllerOpenPattern.Match(mainSearchMatchExam.Value);
                                    // Check is Another Same Named Control Internally Opened Bracket
                                    if (bracketOpenExamMatch.Success &&
                                        string.Compare(directiveID, bracketOpenExamMatch.Result("${ItemID}")) == 0)
                                    {
                                        innerMatch += 1;

                                        continue;
                                    }

                                    // Exam For Separator Bracketed Regex Result
                                    bracketSeparatorExamMatch =
                                        RegularExpression.Current.BracketedControllerSeparatorPattern.Match(mainSearchMatchExam.Value);
                                    // Check is Same Named Highlevel Control Separator Bracket
                                    if (bracketSeparatorExamMatch.Success &&
                                        string.Compare(directiveID, bracketSeparatorExamMatch.Result("${ItemID}")) == 0 &&
                                        innerMatch == 0)
                                    {
                                        // Point the location of Separator Bracket index
                                        separatorIndexes.Add(mainSearchMatchExam.Index - mainSearchMatch.Index);

                                        continue;
                                    }

                                    // Exam For Closing Bracketed Regex Result
                                    bracketCloseExamMatch =
                                        RegularExpression.Current.BracketedControllerClosePattern.Match(mainSearchMatchExam.Value);
                                    // Check is Same Named Control Internally Closed Bracket
                                    if (bracketCloseExamMatch.Success &&
                                        string.Compare(directiveID, bracketCloseExamMatch.Result("${ItemID}")) == 0)
                                    {
                                        if (innerMatch > 0)
                                        {
                                            innerMatch -= 1;

                                            continue;
                                        }

                                        string modifierText = string.Format("~{0}", mainSearchMatch.Index);
                                        string pointedOriginalValue =
                                            rawValue.Substring(mainSearchMatch.Index, (mainSearchMatchExam.Index + mainSearchMatchExam.Length) - mainSearchMatch.Index);

                                        pointedOriginalValue = pointedOriginalValue.Insert(pointedOriginalValue.Length - 1, modifierText);
                                        for (int idxID = separatorIndexes.Count - 1; idxID >= 0; idxID += -1)
                                            pointedOriginalValue = pointedOriginalValue.Insert((separatorIndexes[idxID] + string.Format("}}:{0}", directiveID).Length), modifierText);
                                        pointedOriginalValue = pointedOriginalValue.Insert(mainSearchMatch.Length - 2, modifierText);

                                        IController workingDirective = null;

                                        string directiveRawValue = string.Format("${0}:", (string.IsNullOrEmpty(directiveType) ? directiveID : directiveType));
                                        switch (DirectiveHelper.CaptureDirectiveType(directiveRawValue))
                                        {
                                            case DirectiveTypes.Control:
                                                workingDirective = ControlHelper.MakeControl(mainSearchMatch.Index, pointedOriginalValue, null, this.OnControlResolveRequest);

                                                break;
                                            case DirectiveTypes.InLineStatement:
                                                workingDirective = new InLineStatement(mainSearchMatch.Index, pointedOriginalValue, null);

                                                break;
                                            case DirectiveTypes.UpdateBlock:
                                                workingDirective = new UpdateBlock(mainSearchMatch.Index, pointedOriginalValue, null);

                                                break;
                                            case DirectiveTypes.EncodedExecution:
                                                workingDirective = new EncodedExecution(mainSearchMatch.Index, pointedOriginalValue, null);

                                                break;
                                            case DirectiveTypes.MessageBlock:
                                                workingDirective = new MessageBlock(mainSearchMatch.Index, pointedOriginalValue, null);

                                                break;
                                            case DirectiveTypes.PartialCache:
                                                workingDirective = new PartialCache(mainSearchMatch.Index, pointedOriginalValue, null);

                                                break;
                                        }

                                        if (workingDirective != null)
                                        {
                                            if (workingDirective is IDeploymentAccessRequires)
                                                ((IDeploymentAccessRequires)workingDirective).DeploymentAccessRequested += this.OnDeploymentAccessRequest;

                                            if (workingDirective is IInstanceRequires)
                                                ((IInstanceRequires)workingDirective).InstanceRequested += this.OnInstanceRequest;

                                            childrenContainer.Add(workingDirective);
                                        }

                                        lastIndex = (mainSearchMatchExam.Index + mainSearchMatchExam.Length);

                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            switch (ControllerHelper.CaptureControllerType(mainSearchMatch.Value))
                            {
                                case ControllerTypes.Property:
                                    Property propertyDirective =
                                        new Property(mainSearchMatch.Index, mainSearchMatch.Value, contentArguments);
                                    propertyDirective.InstanceRequested += this.OnInstanceRequest;

                                    childrenContainer.Add(propertyDirective);

                                    break;
                                case ControllerTypes.Directive:
                                    IController workingDirective = null;

                                    switch (DirectiveHelper.CaptureDirectiveType(mainSearchMatch.Value))
                                    {
                                        case DirectiveTypes.Control:
                                            workingDirective = ControlHelper.MakeControl(mainSearchMatch.Index, mainSearchMatch.Value, null, this.OnControlResolveRequest);

                                            break;
                                        case DirectiveTypes.Template:
                                            workingDirective = new Template(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.Translation:
                                            workingDirective = new Translation(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.HashCodePointedTemplate:
                                            workingDirective = new HashCodePointedTemplate(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.Execution:
                                            workingDirective = new Execution(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.InLineStatement:
                                            workingDirective = new InLineStatement(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.UpdateBlock:
                                            workingDirective = new UpdateBlock(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.EncodedExecution:
                                            workingDirective = new EncodedExecution(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                        case DirectiveTypes.PartialCache:
                                            workingDirective = new PartialCache(mainSearchMatch.Index, mainSearchMatch.Value, null);

                                            break;
                                    }

                                    if (workingDirective != null)
                                    {
                                        if (workingDirective is IDeploymentAccessRequires)
                                            ((IDeploymentAccessRequires)workingDirective).DeploymentAccessRequested += this.OnDeploymentAccessRequest;

                                        if (workingDirective is IInstanceRequires)
                                            ((IInstanceRequires)workingDirective).InstanceRequested += this.OnInstanceRequest;

                                        childrenContainer.Add(workingDirective);
                                    }

                                    break;
                            }

                            lastIndex = (mainSearchMatch.Index + mainSearchMatch.Value.Length);
                        }
                    }

                    if (rawValue.Length - lastIndex > 1)
                        childrenContainer.Add(
                            new Renderless(lastIndex, rawValue.Substring(lastIndex), contentArguments));
                }
            }

            private void OnDeploymentAccessRequest(ref Basics.IDomain workingInstance, ref Deployment.DomainDeployment domainDeployment) =>
                domainDeployment = ((Domain)workingInstance)._Deployment;

            private static ConcurrentDictionary<string, ControlSettings> _ControlsCache =
                new ConcurrentDictionary<string, ControlSettings>();
            private void OnControlResolveRequest(string controlID, ref Basics.IDomain workingInstance, out ControlSettings settings)
            {
                settings = null;

                if (string.IsNullOrEmpty(controlID))
                    return;

                if (workingInstance == null)
                    workingInstance = this._Instance;

                do
                {
                    ControlSettings localSettings;

                    string currentDomainIDAccessTreeString =
                        string.Join<string>("-", workingInstance.IDAccessTree);
                    string cacheSearchKey =
                        string.Format("{0}_{1}", currentDomainIDAccessTreeString, controlID);

                    if (Renderer._ControlsCache.TryGetValue(cacheSearchKey, out localSettings))
                    {
                        settings = localSettings.Clone();

                        return;
                    }

                    XPathNavigator controlsXPathNavigator =
                        ((Domain)workingInstance).ControlsXPathNavigator;

                    if (controlsXPathNavigator == null)
                        return;

                    XPathNavigator xPathControlNav =
                        controlsXPathNavigator.SelectSingleNode(string.Format("/Controls/Control[@id='{0}']", controlID));

                    if (xPathControlNav == null || !xPathControlNav.MoveToFirstChild())
                    {
                        workingInstance = workingInstance.Parent;

                        continue;
                    }

                    Dictionary<string, object> controlSettingsDict =
                        new Dictionary<string, object>();
                    CultureInfo compareCulture = new CultureInfo("en-US");

                    do
                    {
                        switch (xPathControlNav.Name.ToLower(compareCulture))
                        {
                            case "type":
                                controlSettingsDict.Add("type", ControlHelper.ParseControlType(xPathControlNav.Value));

                                break;
                            case "bind":
                                controlSettingsDict.Add("bind", Basics.Execution.BindInfo.Make(xPathControlNav.Value));

                                break;
                            case "attributes":
                                XPathNavigator childReader_attr = xPathControlNav.Clone();

                                if (childReader_attr.MoveToFirstChild())
                                {
                                    AttributeInfoCollection AttributesCol =
                                        new AttributeInfoCollection();
                                    do
                                    {
                                        AttributesCol.Add(
                                            childReader_attr.GetAttribute("key", childReader_attr.BaseURI).ToLower(),
                                            childReader_attr.Value
                                        );
                                    } while (childReader_attr.MoveToNext());
                                    controlSettingsDict.Add("attributes", AttributesCol);
                                }

                                break;
                            case "security":
                                XPathNavigator childReader_sec = xPathControlNav.Clone();

                                if (childReader_sec.MoveToFirstChild())
                                {
                                    SecurityInfo Security =
                                        new SecurityInfo();
                                    do
                                    {
                                        switch (childReader_sec.Name.ToLower(compareCulture))
                                        {
                                            case "registeredgroup":
                                                Security.RegisteredGroup = childReader_sec.Value;

                                                break;
                                            case "friendlyname":
                                                Security.FriendlyName = childReader_sec.Value;

                                                break;
                                            case "bind":
                                                Security.Bind = Basics.Execution.BindInfo.Make(childReader_sec.Value);

                                                break;
                                            case "disabled":
                                                Security.Disabled.IsSet = true;
                                                SecurityInfo.DisabledClass.DisabledTypes secType;

                                                if (!Enum.TryParse<SecurityInfo.DisabledClass.DisabledTypes>(
                                                        childReader_sec.GetAttribute("type", childReader_sec.NamespaceURI), out secType))
                                                    secType = SecurityInfo.DisabledClass.DisabledTypes.Inherited;

                                                Security.Disabled.Type = secType;
                                                Security.Disabled.Value = childReader_sec.Value;

                                                break;
                                        }
                                    } while (childReader_sec.MoveToNext());
                                    controlSettingsDict.Add("security", Security);
                                }

                                break;
                            case "blockidstoupdate":
                                bool updateLocalBlock;
                                if (!bool.TryParse(xPathControlNav.GetAttribute("localupdate", xPathControlNav.BaseURI), out updateLocalBlock))
                                    updateLocalBlock = true;
                                controlSettingsDict.Add("blockidstoupdate.localupdate", updateLocalBlock);

                                XPathNavigator childReader_blck = xPathControlNav.Clone();

                                if (childReader_blck.MoveToFirstChild())
                                {
                                    List<string> blockIDsToUpdate = new List<string>();
                                    do
                                    {
                                        blockIDsToUpdate.Add(childReader_blck.Value);
                                    } while (childReader_blck.MoveToNext());
                                    controlSettingsDict.Add("blockidstoupdate", blockIDsToUpdate);
                                }

                                break;
                            case "defaultbuttonid":
                                controlSettingsDict.Add("defaultbuttonid", xPathControlNav.Value);

                                break;
                            case "text":
                                controlSettingsDict.Add("text", xPathControlNav.Value);

                                break;
                            case "url":
                                controlSettingsDict.Add("url", xPathControlNav.Value);

                                break;
                            case "content":
                                controlSettingsDict.Add("content", xPathControlNav.Value);

                                break;
                            case "source":
                                controlSettingsDict.Add("source", xPathControlNav.Value);

                                break;
                        }
                    } while (xPathControlNav.MoveToNext());

                    localSettings = new ControlSettings(controlSettingsDict);
                    Renderer._ControlsCache.TryAdd(cacheSearchKey, localSettings);

                    settings = localSettings.Clone();

                    return;
                } while (workingInstance != null);
            }

            private void OnInstanceRequest(ref Basics.IDomain instance) =>
                instance = this._Instance;

            public void ClearCache() =>
                Renderer._ControlsCache.Clear();
        }
    }
}