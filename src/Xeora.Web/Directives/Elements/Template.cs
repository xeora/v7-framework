using System;
using System.Collections.Generic;
using System.Security;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Deployment;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Template : Directive, INamable, IHasChildren
    {
        private DirectiveCollection _Children;
        private bool _Authenticated;
        private bool _Parsed;

        public Template(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Template, arguments)
        {
            this.DirectiveID = DirectiveHelper.CaptureDirectiveID(rawValue);
        }

        public string DirectiveID { get; private set; }

        public override bool Searchable => true;
        public override bool CanAsync => true;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            IDomain workingInstance = instance;

            this._Authenticated = 
                this.CheckAuthentication(ref instance, ref workingInstance);

            if (!this._Authenticated)
                return;

            this.ParseInternal(ref workingInstance);
        }

        private void ParseInternal(ref IDomain workingInstance)
        {
            string templateContent =
                this.LoadTemplate(ref workingInstance);

            // Template does not have any Arguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Mother.RequestParsing(templateContent, ref this._Children, this.Arguments);
        }

        private string LoadTemplate(ref IDomain workingInstance)
        {
            Domain deployment = null;
            this.Mother.RequestDeploymentAccess(ref workingInstance, ref deployment);

            if (deployment == null)
                throw new System.Exception("Domain Deployment access is failed!");

            return deployment.ProvideTemplateContent(this.DirectiveID);
        }

        private bool CheckAuthentication(ref IDomain originalInstance, ref IDomain workingInstance)
        {
            // Gather Parent Authentication Keys
            List<string> authenticationKeys = new List<string>();
            IServiceItem serviceItem = null;
            IDomain templateInstance = null;

            IServiceItem cachedServiceItem = serviceItem;
            while (workingInstance != null)
            {
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveID);

                if (cachedServiceItem != null)
                {
                    if (serviceItem == null)
                    {
                        serviceItem = cachedServiceItem;
                        templateInstance = workingInstance;
                    }

                    foreach (string key in cachedServiceItem.AuthenticationKeys)
                    {
                        bool isExists = false;
                        foreach (string item in authenticationKeys)
                        {
                            if (string.Compare(item, key, true) == 0)
                            {
                                isExists = true;

                                break;
                            }
                        }
                        if (!isExists)
                            authenticationKeys.Add(key);
                    }
                }

                workingInstance = workingInstance.Parent;
            }

            if (serviceItem == null)
                throw new SecurityException(string.Format("Service definition of {0} has not been found!", this.DirectiveID));

            serviceItem.AuthenticationKeys = authenticationKeys.ToArray();

            // Search for Overriding Children
            cachedServiceItem = serviceItem;
            workingInstance = originalInstance;

            while (cachedServiceItem.Overridable)
            {
                workingInstance = this.SearchChildrenThatOverrides(ref originalInstance, ref workingInstance);

                // If not null, it means WorkingInstance contains a service definition which will override
                if (workingInstance == null)
                    break;

                originalInstance = workingInstance;
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveID);

                // Merge or set the authenticationkeys
                if (cachedServiceItem.Authentication)
                {
                    if (cachedServiceItem.AuthenticationKeys.Length == 0)
                        cachedServiceItem.AuthenticationKeys = serviceItem.AuthenticationKeys;
                    else
                    {
                        // Merge
                        string[] Keys = new string[cachedServiceItem.AuthenticationKeys.Length + serviceItem.AuthenticationKeys.Length];

                        Array.Copy(cachedServiceItem.AuthenticationKeys, 0, Keys, 0, cachedServiceItem.AuthenticationKeys.Length);
                        Array.Copy(serviceItem.AuthenticationKeys, 0, Keys, cachedServiceItem.AuthenticationKeys.Length, serviceItem.AuthenticationKeys.Length);

                        cachedServiceItem.AuthenticationKeys = Keys;
                    }
                }

                serviceItem = cachedServiceItem;
            }
            // !---
            if (workingInstance == null || object.ReferenceEquals(workingInstance, originalInstance))
                workingInstance = templateInstance;

            if (!serviceItem.Authentication)
                return true;

            bool localAuthenticationNotAccepted = false;

            foreach (string authKey in serviceItem.AuthenticationKeys)
            {
                if (Basics.Helpers.Context.Session[authKey] == null)
                {
                    localAuthenticationNotAccepted = true;

                    break;
                }
            }

            if (!localAuthenticationNotAccepted)
                return true;

            return false;
        }

        private IDomain SearchChildrenThatOverrides(ref IDomain originalInstance, ref IDomain workingInstance)
        {
            if (workingInstance == null)
                return null;

            List<string> childDomainIDAccessTree = new List<string>();
            childDomainIDAccessTree.AddRange(workingInstance.IDAccessTree);

            foreach (Basics.Domain.Info.Domain childDI in workingInstance.Children)
            {
                childDomainIDAccessTree.Add(childDI.ID);

                IDomain rDomainInstance = new Site.Domain(childDomainIDAccessTree.ToArray(), originalInstance.Languages.Current.Info.ID);
                IServiceItem serviceItem =
                    rDomainInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveID);

                if (serviceItem == null ||
                    serviceItem.ServiceType != ServiceTypes.Template)
                {
                    if (rDomainInstance.Children.Count > 0)
                    {
                        rDomainInstance = this.SearchChildrenThatOverrides(ref originalInstance, ref rDomainInstance);

                        if (rDomainInstance != null)
                            return rDomainInstance;
                    }
                }
                else
                    return rDomainInstance;

                childDomainIDAccessTree.RemoveAt(childDomainIDAccessTree.Count - 1);
            }

            return null;
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            if (!this._Authenticated)
            {
                this.Deliver(
                    RenderStatus.Rendered,
                    "<div style='width:100%; font-weight:bolder; color:#CC0000; text-align:center'>" + SystemMessages.TEMPLATE_AUTH + "!</div>"
                );
                return;
            }

            this.Children.Render(this.UniqueID);
            this.Deliver(RenderStatus.Rendered, this.Result);

            this.Mother.Pool.Register(this);
            this.Mother.Scheduler.Fire(this.DirectiveID);
        }
    }
}