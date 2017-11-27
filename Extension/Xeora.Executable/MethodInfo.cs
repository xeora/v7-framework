namespace Xeora.Extension.Executable
{
    public class MethodInfo
    {
        public MethodInfo(string ID, string[] @params)
        {
            this.ID = ID;
            this.Params = @params;
        }

        public string ID { get; private set; }
        public string[] Params { get; private set; }
    }
}
