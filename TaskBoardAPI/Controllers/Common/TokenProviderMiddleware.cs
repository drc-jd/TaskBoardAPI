using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Data;
using TaskBoardAPI.Utils;
using System.Data.SqlClient;

namespace TaskBoardAPI.Controllers.Common
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _serializerSettings;
        readonly ILogger<TokenProviderMiddleware> logger;

        public TokenProviderMiddleware(RequestDelegate next, ILogger<TokenProviderMiddleware> logger)
        {
            _next = next;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            this.logger = logger;
        }

        public System.Threading.Tasks.Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals("/api/token", StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            var token = GenerateToken(context);
            return token;
        }
        private async System.Threading.Tasks.Task GenerateToken(HttpContext context)
        {
            // Serialize and return the response
            string client_id = context.Request.Form["client_id"];
            string UserName = context.Request.Form["UserName"];
            string Password = context.Request.Form["Password"];

            DataTable tblData = new DataTable();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("UserName",UserName),
                    new SqlParameter("Password",Password)
                };
                tblData = await SqlHelper.GetDatatableSP("usp_TaskUserLogin", SqlHelper.ConnectionString, objparam);

            }

            if (tblData == null || tblData.Rows.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject("Invalid Username or Password", _serializerSettings));
                return;
            }

            var response = GetLoginToken.Execute(tblData,client_id);
            //var response = new { };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
        }
    }
}
