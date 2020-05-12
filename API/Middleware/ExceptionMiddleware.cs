using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;

        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                if (_env.IsDevelopment() == true)
                {
                    var response = new ApiException((int)HttpStatusCode.InternalServerError, ex.Message,
                        ex.StackTrace.ToString());
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);

                }
                else
                {
                    var response = new ApiResponse((int)HttpStatusCode.InternalServerError);
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);
                }
            };

        }
    }
}