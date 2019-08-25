using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Managers;
using Microsoft.AspNetCore.Http;

namespace Hetacode.Microless.Middlewares
{
    public class HttpCallFunctionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpManager _httpManager;

        public HttpCallFunctionMiddleware(RequestDelegate next, IHttpManager httpManager)
            => (_next, _httpManager) = (next, httpManager);

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine(context.Request.Path);
            var result = "";
            using (var reader = new StreamReader(context.Request.Body))
            {
                result = await reader.ReadToEndAsync();
            }

            _httpManager.ResolveAndCall(context.Request.Path, result, context.Request.Headers.ToDictionary(t => t.Key, t => t.Value.FirstOrDefault()));
        }
    }
}
