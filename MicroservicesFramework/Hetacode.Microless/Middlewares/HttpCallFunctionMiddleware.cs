using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hetacode.Microless.Abstractions.Managers;
using Hetacode.Microless.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
            _httpManager.ResolveAndCall(context.Request.Path,
                                        result,
                                        context.Request.Headers.ToDictionary(t => t.Key, t => t.Value.FirstOrDefault()),
                                        async (response, headers) =>
            {
                if (headers != null)
                {

                    headers.ToList().ForEach(f =>
                    {
                        HeaderDictionaryExtensions.Append(context.Response.Headers, f.Key, f.Value);
                    });
                    context.Response.Headers.Remove("Content-Length");
                }
                var type = response.GetType().AssemblyQualifiedName;
                var dynamicMessage = response.ToExpandoObject();
                dynamicMessage.TryAdd("_type", type);
                var json = JsonConvert.SerializeObject(dynamicMessage);
                await context.Response.WriteAsync(json);
            });
        }
    }
}
