using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Listjj.Middleware
{
    public class TokenToHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authorization = context.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"Original Authorization: {authorization}");

            if (string.IsNullOrEmpty(authorization))
            {
                var token = context.Request.Cookies["token"];
                Console.WriteLine($"Token from cookie: {token}");
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Authorization = $"Bearer {token}";
                    Console.WriteLine($"New Authorization: {context.Request.Headers["Authorization"]}");
                }
            }

            await _next(context);
        }
    }
}
