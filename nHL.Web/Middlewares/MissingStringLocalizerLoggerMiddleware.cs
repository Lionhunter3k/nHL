using Microsoft.AspNetCore.Http;
using nHL.Web.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace nHL.Web.Middlewares
{
    public class MissingStringLocalizerLoggerMiddleware : IMiddleware
    {
        private readonly IMissingStringLocalizerLogger missingStringLocalizerLogger;

        public MissingStringLocalizerLoggerMiddleware(IMissingStringLocalizerLogger missingStringLocalizerLogger)
        {
            this.missingStringLocalizerLogger = missingStringLocalizerLogger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);
            await missingStringLocalizerLogger.FlushMissingStringsAsync();
        }
    }
}
