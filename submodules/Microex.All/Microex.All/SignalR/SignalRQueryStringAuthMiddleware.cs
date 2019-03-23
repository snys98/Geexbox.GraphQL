using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microex.All.SignalR
{
    public class SignalRQueryStringAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private string _queryName;

        public SignalRQueryStringAuthMiddleware(RequestDelegate next, string queryName)
        {
            _next = next;
            _queryName = queryName;
        }

        // Convert incomming qs auth token to a Authorization header so the rest of the chain
        // can authorize the request correctly
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["Connection"] == "Upgrade" &&
                context.Request.Query.TryGetValue(_queryName, out var token))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token.First());
            }
            await _next.Invoke(context);
        }
    }

    public static class SignalRQueryStringAuthExtensions
    {
        public static IApplicationBuilder UseSignalRQueryStringAuth(this IApplicationBuilder builder, string queryName = "access_token")
        {
            return builder.UseMiddleware<SignalRQueryStringAuthMiddleware>(queryName);
        }
    }
}
