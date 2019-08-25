using System;
using System.Collections.Generic;

namespace Hetacode.Microless.Abstractions.Managers
{
    public interface IHttpManager
    {
        void Init();

        void ResolveAndCall(string endpoint,
                            string body,
                            Dictionary<string, string> headers,
                            Action<object, Dictionary<string, string>> responseAction);
    }
}
