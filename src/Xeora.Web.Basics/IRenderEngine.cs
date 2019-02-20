namespace Xeora.Web.Basics
{
    public interface IRenderEngine
    {
        RenderResult Render(ServiceDefinition serviceDefinition, ControlResult.Message messageResult, string[] updateBlockControlIDStack = null);
        RenderResult Render(string xeoraContent, ControlResult.Message messageResult, string[] updateBlockControlIDStack = null);
        void ClearCache();
    }
}
