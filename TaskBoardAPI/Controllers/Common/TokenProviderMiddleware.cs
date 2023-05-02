using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
            string client_id = context.Request.Form["client_id"];
            string UserName = context.Request.Form["UserName"];
            string Password = context.Request.Form["Password"];
            string clientId = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            DataTable tblData = new DataTable();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                string hash = Utilities.GetMd5HashWithMySecurityAlgo(Password);
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("UserName",UserName),
                    new SqlParameter("Password",hash)
                };
                if (clientId.Equals("::1"))
                    tblData = await SqlHelper.GetDataTable("Select u.UserId, u.UserName, u.FirstName, u.LastName,ISNULL(u.ProfileImg, '') ProfileImg,u.Role From Task_UserList u  Where u.UserName = '" + UserName + "'", SqlHelper.ConnectionString);
                else
                    tblData = await SqlHelper.GetDatatableSP("usp_TaskUserLogin", SqlHelper.ConnectionString, objparam);
            }

            if (tblData == null || tblData.Rows.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject("Invalid Username or Password", _serializerSettings));
                return;
            }

            var response = GetLoginToken.Execute(tblData, client_id);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
        }
    }
}
