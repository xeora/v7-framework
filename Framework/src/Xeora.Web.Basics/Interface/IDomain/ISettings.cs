using System;

namespace Xeora.Web.Basics
{
    public interface ISettings : IDisposable
    {
        IConfigurations Configurations { get; }
        IServices Services { get; }
        IURLMappings URLMappings { get; }
    }
}
