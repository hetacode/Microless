using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IFunctionsManager
    {
        Task CallFunction<T>(T message, Dictionary<string, string> headers = null);

        void ScaffoldFunctions();
    }
}