using Microsoft.AspNetCore.Http;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Middlewares
{
    public class MissingStringLocalizerLoggerMiddleware
    {
        private readonly RequestDelegate next;

        public MissingStringLocalizerLoggerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMissingStringLocalizerLogger missingStringLocalizerLogger)
        {
            await next(context);
            await missingStringLocalizerLogger.FlushMissingStringsAsync();
        }
    }
}
