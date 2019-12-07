using System;
using System.Collections.Generic;
using System.Security;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Deployment;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Template : Directive, INameable
    {
        private bool _Authenticated;
        private bool _Parsed;

        public Template(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Template, arguments) =>
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);

        public string DirectiveId { get; }

        public override bool Searchable => true;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => false;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;
            
            this.Mother.RequestInstance(out IDomain instance);

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

            // Template needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(templateContent, this.Children, this.Arguments);
        }

        private string LoadTemplate(ref IDomain workingInstance)
        {
            this.Mother.RequestDeploymentAccess(ref workingInstance, out Domain deployment);

            if (deployment == null)
                throw new Exception("Domain Deployment access is failed!");

            return deployment.ProvideTemplateContent(this.DirectiveId);
        }

        private bool CheckAuthentication(ref IDomain originalInstance, ref IDomain workingInstance)
        {
            // Gather Parent Authentication Keys
            List<string> authenticationKeys = new List<string>();
            IServiceItem serviceItem = null;
            IDomain templateInstance = null;

            IServiceItem cachedServiceItem;
            while (workingInstance != null)
            {
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveId);

                if (cachedServiceItem == null)
                {
                    workingInstance = workingInstance.Parent;
                    continue;
                }
                
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
                        if (string.Compare(item, key, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        
                        isExists = true;
                        break;
                    }
                    if (!isExists)
                        authenticationKeys.Add(key);
                }
                
                workingInstance = workingInstance.Parent;
            }

            if (serviceItem == null)
                throw new SecurityException($"Service definition of {this.DirectiveId} has not been found!");

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
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveId);

                // Merge or set the authenticationKeys
                if (cachedServiceItem.Authentication)
                {
                    if (cachedServiceItem.AuthenticationKeys.Length == 0)
                        cachedServiceItem.AuthenticationKeys = serviceItem.AuthenticationKeys;
                    else
                    {
                        // Merge
                        string[] keys = new string[cachedServiceItem.AuthenticationKeys.Length + serviceItem.AuthenticationKeys.Length];

                        Array.Copy(cachedServiceItem.AuthenticationKeys, 0, keys, 0, cachedServiceItem.AuthenticationKeys.Length);
                        Array.Copy(serviceItem.AuthenticationKeys, 0, keys, cachedServiceItem.AuthenticationKeys.Length, serviceItem.AuthenticationKeys.Length);

                        cachedServiceItem.AuthenticationKeys = keys;
                    }
                }

                serviceItem = cachedServiceItem;
            }
            // !---
            if (workingInstance == null || ReferenceEquals(workingInstance, originalInstance))
                workingInstance = templateInstance;

            if (!serviceItem.Authentication)
                return true;

            bool localAuthenticationNotAccepted = false;

            foreach (string authKey in serviceItem.AuthenticationKeys)
            {
                if (Basics.Helpers.Context.Session[authKey] != null) continue;
                
                localAuthenticationNotAccepted = true;
                break;
            }

            return !localAuthenticationNotAccepted;
        }

        private IDomain SearchChildrenThatOverrides(ref IDomain originalInstance, ref IDomain workingInstance)
        {
            if (workingInstance == null)
                return null;

            List<string> childDomainIdAccessTree = new List<string>();
            childDomainIdAccessTree.AddRange(workingInstance.IdAccessTree);

            foreach (Basics.Domain.Info.Domain childDI in workingInstance.Children)
            {
                childDomainIdAccessTree.Add(childDI.Id);

                IDomain rDomainInstance = 
                    new Application.Domain(childDomainIdAccessTree.ToArray(), originalInstance.Languages.Current.Info.Id);
                IServiceItem serviceItem =
                    rDomainInstance.Settings.Services.ServiceItems.GetServiceItem(this.DirectiveId);

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

                childDomainIdAccessTree.RemoveAt(childDomainIdAccessTree.Count - 1);
            }

            return null;
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            if (this._Authenticated) return true;
            
            this.Deliver(
                RenderStatus.Rendered,
                "<div style='width:100%; font-weight:bolder; color:#CC0000; text-align:center'>" + SystemMessages.TEMPLATE_AUTH + "!</div>"
            );
            
            return false;
        }
        
        public override void PostRender()
        {
            this.Deliver(RenderStatus.Rendered, this.Result);
            this.Scheduler.Fire();
        }
    }
}