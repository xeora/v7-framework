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
    public class Domain : Basics.Domain.IDomain
    {
        private Renderer _Renderer;
        private Deployment.Domain _Deployment;
        private LanguagesHolder _LanguagesHolder;

        public Domain(string[] domainIDAccessTree) :
            this(domainIDAccessTree, null)
        { }

        public Domain(string[] domainIDAccessTree, string languageID) =>
            this.BuildDomain(domainIDAccessTree, languageID);

        private void BuildDomain(string[] domainIDAccessTree, string languageID)
        {
            if (domainIDAccessTree == null)
                domainIDAccessTree = Basics.Configurations.Xeora.Application.Main.DefaultDomain;

            try
            {
                this._Deployment = Deployment.InstanceFactory.Current.GetOrCreate(domainIDAccessTree);
            }
            catch (Exception.DomainNotExistsException)
            {
                // Try with the default one if requested one is not the default one
                if (string.Compare(
                    string.Join("\\", domainIDAccessTree),
                    string.Join("\\", Basics.Configurations.Xeora.Application.Main.DefaultDomain)) != 0)
                    this._Deployment =
                        Deployment.InstanceFactory.Current.GetOrCreate(
                            Basics.Configurations.Xeora.Application.Main.DefaultDomain);
                else
                    throw;
            }
            catch (System.Exception)
            {
                throw;
            }

            if (domainIDAccessTree.Length > 1)
            {
                string[] ParentDomainIDAccessTree = new string[domainIDAccessTree.Length - 1];
                Array.Copy(domainIDAccessTree, 0, ParentDomainIDAccessTree, 0, ParentDomainIDAccessTree.Length);

                this.Parent = new Domain(ParentDomainIDAccessTree);
            }

            this._LanguagesHolder = new LanguagesHolder(this, this._Deployment.Languages);
            this._LanguagesHolder.Use(languageID);

            if (this._Renderer == null)
                this._Renderer = new Renderer();

            this._Renderer.Inject(this);
        }

        public void SetLanguageChangedListener(Action<Basics.Domain.ILanguage> languageChangedListener) =>
            this._LanguagesHolder.LanguageChangedListener = languageChangedListener;

        public string[] IDAccessTree => this._Deployment.DomainIDAccessTree;
        public Basics.Domain.Info.DeploymentTypes DeploymentType => this._Deployment.DeploymentType;

        public Basics.Domain.IDomain Parent { get; private set; }
        public Basics.Domain.Info.DomainCollection Children => this._Deployment.Children;

        public string ContentsVirtualPath =>
            string.Format(
                "{0}{1}_{2}",
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation,
                string.Join<string>("-", this._Deployment.DomainIDAccessTree),
                this._LanguagesHolder.Current.Info.ID
            );

        public Basics.Domain.ISettings Settings => this._Deployment.Settings;
        public Basics.Domain.ILanguages Languages => this._LanguagesHolder;
        public Basics.Domain.IxService xService => this._Deployment.xService;

        public void ProvideFileStream(string requestedFilePath, out Stream outputStream)
        {
            outputStream = null;
            try
            {
                this._Deployment.ProvideContentFileStream(
                    this.Languages.Current.Info.ID,
                    requestedFilePath,
                    out outputStream
                );
            }
            catch (FileNotFoundException)
            {
                if (this.Parent == null)
                    throw;
            }

            if (outputStream == null && this.Parent != null)
                this.Parent.ProvideFileStream(requestedFilePath, out outputStream);
        }

        public void ClearCache()
        {
            Master.ClearCache();

            this._Renderer.ClearCache();
            PartialCache.ClearCache(this.IDAccessTree);

            Deployment.InstanceFactory.Current.Reset();
        }

        public Basics.Domain.RenderResult Render(Basics.ServiceDefinition serviceDefinition, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null) =>
            this._Renderer.Start(serviceDefinition, messageResult, updateBlockControlIDStack);

        public Basics.Domain.RenderResult Render(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null) =>
            this._Renderer.Start(xeoraContent, messageResult, updateBlockControlIDStack);

        private class Renderer
        {
            private Basics.Domain.IDomain _Instance = null;

            public void Inject(Basics.Domain.IDomain instance) =>
                this._Instance = instance;

            public Basics.Domain.RenderResult Start(Basics.ServiceDefinition serviceDefinition, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null) =>
                this.Start(string.Format("$T:{0}$", serviceDefinition.FullPath), messageResult, updateBlockControlIDStack);

            public Basics.Domain.RenderResult Start(string xeoraContent, Basics.ControlResult.Message messageResult, string[] updateBlockControlIDStack = null)
            {
                if (this._Instance == null)
                    throw new System.Exception("Injection required!");

                Mother ExternalController =
                    new Mother(0, xeoraContent, messageResult, updateBlockControlIDStack);
                ExternalController.ParseRequested += this.OnParseRequest;
                ExternalController.Setup();

                ExternalController.Render(null);

                return new Basics.Domain.RenderResult(ExternalController.RenderedValue, ExternalController.HasInlineError);
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

                        // Exam For Bracketed Regex Result
                        bracketOpenExamMatch = RegularExpression.Current.BracketedControllerOpenPattern.Match(mainSearchMatch.Value);
                        if (bracketOpenExamMatch.Success &&
                            !string.IsNullOrEmpty(bracketOpenExamMatch.Result("${DirectiveIndex}"))) continue;

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
                                            case DirectiveTypes.FormattableTranslation:
                                                workingDirective = new FormattableTranslation(mainSearchMatch.Index, pointedOriginalValue, null);

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
                                        case DirectiveTypes.FormattableTranslation:
                                            workingDirective = new FormattableTranslation(mainSearchMatch.Index, mainSearchMatch.Value, null);

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

            private void OnDeploymentAccessRequest(ref Basics.Domain.IDomain workingInstance, ref Deployment.Domain deployment) =>
                deployment = ((Domain)workingInstance)._Deployment;

            private static ConcurrentDictionary<string, ControlSettings> _ControlsCache =
                new ConcurrentDictionary<string, ControlSettings>();
            private void OnControlResolveRequest(string controlID, ref Basics.Domain.IDomain workingInstance, out ControlSettings settings)
            {
                settings = null;

                if (string.IsNullOrEmpty(controlID))
                    return;

                if (workingInstance == null)
                    workingInstance = this._Instance;

                do
                {
                    Basics.Domain.Info.DeploymentTypes deploymentType =
                        ((Domain)workingInstance)._Deployment.DeploymentType;

                    string currentDomainIDAccessTreeString =
                        string.Join<string>("-", workingInstance.IDAccessTree);
                    string cacheSearchKey =
                        string.Format("{0}_{1}", currentDomainIDAccessTreeString, controlID);

                    if (deploymentType == Basics.Domain.Info.DeploymentTypes.Release && 
                        Renderer._ControlsCache.TryGetValue(cacheSearchKey, out ControlSettings localSettings))
                    {
                        settings = localSettings.Clone();

                        return;
                    }

                    localSettings = new ControlSettings();

                    XPathNavigator xPathControlNav =
                        ((Domain)workingInstance)._Deployment.Controls.Select(controlID);

                    if (xPathControlNav == null || !xPathControlNav.MoveToFirstChild())
                    {
                        workingInstance = workingInstance.Parent;

                        continue;
                    }

                    CultureInfo compareCulture = new CultureInfo("en-US");
                    do
                    {
                        switch (xPathControlNav.Name.ToLower(compareCulture))
                        {
                            case "type":
                                localSettings.Type = ControlHelper.ParseControlType(xPathControlNav.Value);

                                break;
                            case "bind":
                                localSettings.Bind = Basics.Execution.Bind.Make(xPathControlNav.Value);

                                break;
                            case "attributes":
                                XPathNavigator childReader_attr = xPathControlNav.Clone();

                                if (childReader_attr.MoveToFirstChild())
                                {
                                    do
                                    {
                                        localSettings.Attributes.Add(
                                            childReader_attr.GetAttribute("key", childReader_attr.BaseURI).ToLower(),
                                            childReader_attr.Value
                                        );
                                    } while (childReader_attr.MoveToNext());
                                }

                                break;
                            case "security":
                                XPathNavigator childReader_sec = xPathControlNav.Clone();

                                if (childReader_sec.MoveToFirstChild())
                                {
                                    do
                                    {
                                        switch (childReader_sec.Name.ToLower(compareCulture))
                                        {
                                            case "registeredgroup":
                                                localSettings.Security.RegisteredGroup = childReader_sec.Value;

                                                break;
                                            case "friendlyname":
                                                localSettings.Security.FriendlyName = childReader_sec.Value;

                                                break;
                                            case "bind":
                                                localSettings.Security.Bind = Basics.Execution.Bind.Make(childReader_sec.Value);

                                                break;
                                            case "disabled":
                                                localSettings.Security.Disabled.Set = true;

                                                SecurityDefinition.DisabledDefinition.Types secType;
                                                if (!Enum.TryParse<SecurityDefinition.DisabledDefinition.Types>(
                                                        childReader_sec.GetAttribute("type", childReader_sec.NamespaceURI), out secType))
                                                    secType = SecurityDefinition.DisabledDefinition.Types.Inherited;

                                                localSettings.Security.Disabled.Type = secType;
                                                localSettings.Security.Disabled.Value = childReader_sec.Value;

                                                break;
                                        }
                                    } while (childReader_sec.MoveToNext());
                                }

                                break;
                            case "blockidstoupdate":
                                bool updateLocalBlock;
                                if (!bool.TryParse(xPathControlNav.GetAttribute("localupdate", xPathControlNav.BaseURI), out updateLocalBlock))
                                    updateLocalBlock = true;
                                localSettings.UpdateLocalBlock = updateLocalBlock;

                                XPathNavigator childReader_blck = xPathControlNav.Clone();

                                if (childReader_blck.MoveToFirstChild())
                                {
                                    List<string> blockIDsToUpdate = new List<string>();
                                    do
                                    {
                                        blockIDsToUpdate.Add(childReader_blck.Value);
                                    } while (childReader_blck.MoveToNext());
                                    localSettings.BlockIDsToUpdate = blockIDsToUpdate.ToArray();
                                }

                                break;
                            case "defaultbuttonid":
                                localSettings.DefaultButtonID = xPathControlNav.Value;

                                break;
                            case "text":
                                localSettings.Text = xPathControlNav.Value;

                                break;
                            case "url":
                                localSettings.URL = xPathControlNav.Value;

                                break;
                            case "content":
                                localSettings.Content = xPathControlNav.Value;

                                break;
                            case "source":
                                localSettings.Source = xPathControlNav.Value;

                                break;
                        }
                    } while (xPathControlNav.MoveToNext());

                    Renderer._ControlsCache.TryAdd(cacheSearchKey, localSettings);

                    settings = localSettings.Clone();

                    return;
                } while (workingInstance != null);
            }

            private void OnInstanceRequest(ref Basics.Domain.IDomain instance) =>
                instance = this._Instance;

            public void ClearCache() =>
                Renderer._ControlsCache.Clear();
        }
    }
}