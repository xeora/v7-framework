namespace Xeora.CLI.Basics
{
    public interface ICommand
    {
        void PrintUsage();
        int SetArguments(string[] args);
        int Execute();
    }
}
