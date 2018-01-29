using System;
using System.Collections.Generic;
using System.Security;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Deployment;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive
{
    public class Template : DirectiveWithChildren, INamable, IDeploymentAccessRequires, IInstanceRequires
    {
        public event InstanceHandler InstanceRequested;
        public event DeploymentAccessHandler DeploymentAccessRequested;

        public Template(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.Template, contentArguments)
        {
            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
        }

        public string ControlID { get; private set; }

        public override void Render(string requesterUniqueID)
        {
            // Template should always included with security check in render process in UpdateBlockRequest because
            // UpdateBlock can be located under a template included in another template

            IDomain instance = null;
            InstanceRequested?.Invoke(ref instance);

            IDomain workingInstance = instance;

            if (!this.CheckIsAuthenticated(ref instance, ref workingInstance))
            {
                this.RenderedValue = "<div style='width:100%; font-weight:bolder; color:#CC0000; text-align:center'>" + SystemMessages.TEMPLATE_AUTH + "!</div>";

                return;
            }

            this.RenderInternal(ref workingInstance, requesterUniqueID);
        }

        public override IController Find(string controlID)
        {
            IDomain instance = null;
            InstanceRequested?.Invoke(ref instance);

            IDomain workingInstance = instance;

            if (!this.CheckIsAuthenticated(ref instance, ref workingInstance))
                return null;

            string templateContent =
                this.LoadTemplate(ref workingInstance);

            this.RenderedValue = templateContent;

            return base.Find(controlID);
        }

        private bool CheckIsAuthenticated(ref IDomain originalInstance, ref IDomain workingInstance)
        {
            // Gather Parent Authentication Keys
            List<string> authenticationKeys = new List<string>();
            IServiceItem serviceItem = null;
            IDomain templateInstance = null;

            IServiceItem cachedServiceItem = serviceItem;
            while (workingInstance != null)
            {
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ControlID);

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
                throw new SecurityException(string.Format("Service definition of {0} has not been found!", this.ControlID));

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
                cachedServiceItem = workingInstance.Settings.Services.ServiceItems.GetServiceItem(this.ControlID);

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
                    rDomainInstance.Settings.Services.ServiceItems.GetServiceItem(this.ControlID);

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

        private string LoadTemplate(ref IDomain workingInstance)
        {
            // Template does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            Domain deployment = null;
            DeploymentAccessRequested?.Invoke(ref workingInstance, ref deployment);

            if (deployment == null)
                throw new System.Exception("Domain Deployment access is failed!");

            return deployment.ProvideTemplateContent(this.ControlID);
        }

        private void RenderInternal(ref IDomain workingInstance, string requesterUniqueID)
        {
            string templateContent =
                this.LoadTemplate(ref workingInstance);

            this.Parse(templateContent);
            base.Render(requesterUniqueID);
        }
    }
}