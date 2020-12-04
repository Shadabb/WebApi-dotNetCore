using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreCodeCamp.Middleware
{
    public class FeatureSwitchMiddleware
    {
        private readonly RequestDelegate _next;

        public FeatureSwitchMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IConfiguration config)
        {

            if (httpContext.Request.Path.Value.Contains("/Camps"))
            {
                //var switches = config.GetSection("FeatureSwitches");

                //var report = switches.GetChildren().Select(x = $"{x.key}: {x.value}");

                //await httpContext.Response.WriteAsync(string.Join("\n", report));
                await httpContext.Response.WriteAsync("The middleware implementation");


            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
