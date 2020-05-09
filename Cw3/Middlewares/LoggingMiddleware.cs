using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string query = httpContext.Request.Path;
                string qstring = httpContext.Request?.QueryString.ToString();
                string method = httpContext.Request.Method.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                StreamWriter sw = new StreamWriter("requestsLog.txt");
                sw.WriteLine("query:" + query);
                sw.WriteLine("qstring:" + qstring);
                sw.WriteLine("method:" + method);
                sw.WriteLine("bodyStr:" + bodyStr);
                sw.WriteLine();
            }
            await _next(httpContext);
        }
    }
}
