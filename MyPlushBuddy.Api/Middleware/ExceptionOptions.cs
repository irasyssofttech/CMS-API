using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyPlushBuddy.Api.Models;
using System;

namespace MyPlushBuddy.Api.Middleware
{
    public class ApiExceptionOptions
    {
        public Action<HttpContext, Exception, ApiErrorModel> AddResponseDetails { get; set; }
        public Func<Exception, LogLevel> DetermineLogLevel { get; set; }
    }
}
