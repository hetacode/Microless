using System.Threading.Tasks;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IFunctionsManager
    {
        Task CallFunction<T>(T message);

        void ScaffoldFunctions();
    }
}