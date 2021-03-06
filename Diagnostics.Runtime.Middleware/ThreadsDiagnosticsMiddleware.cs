﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Runtime;

namespace Diagnostics.Runtime.Middleware
{
    internal class ThreadsDiagnosticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDataTargetProvider _dataTargetProvider;

        public ThreadsDiagnosticsMiddleware(RequestDelegate next, IDataTargetProvider dataTargetProvider)
        {
            _next = next;
            _dataTargetProvider = dataTargetProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ClrInfo runtimeInfo = _dataTargetProvider.GetDataTarget().ClrVersions[0];
            ClrRuntime runtime = runtimeInfo.CreateRuntime();
            var content = TableBuilder.CreateDataTable("Threads", runtime.Threads.Select(f => new
            {
                ThreadId = f.OSThreadId,
                GcMode = f.GcMode,
                Runtime = f.Runtime.ToString(),
            }));

            await _next(context);
            await context.Response.WriteAsync(content);
        }
    }
}
