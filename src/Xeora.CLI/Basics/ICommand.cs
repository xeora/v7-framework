using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xeora.CLI.Basics
{
    public interface ICommand
    {
        Task<int> Execute(IReadOnlyList<string> args);
    }
}
