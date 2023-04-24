using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("Data")]
        public async Task<ApiResponse> Data(dynamic paramList)
        {
            try
            {
                ApiResponse response = new ApiResponse();
                switch (pub.GetString(paramList.Type).ToUpper())
                {
                    case "LOGIN":
                        response = await Login(pub.GetString(paramList.UserName), pub.GetString(paramList.Password));
                        break;
                    case "CHANGEPASSWORD":
                        response = await ChangePassword(pub.GetInt(paramList.UserId), pub.GetString(paramList.CurrentPassword), pub.GetString(paramList.NewPassword));
                        break;
                    case "GETIMAGEBYID":
                        response = await GetImageById(pub.GetInt(paramList.UserId));
                        break;
                    case "GETUSERPERMISSION":
                        response = await GetUserpermission(pub.GetInt(paramList.UserId));
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> Login(string UserName, string Password)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("UserName",UserName),
                    new SqlParameter("Password",Password)
                };
                DataTable tblData = await SqlHelper.GetDatatableSP("usp_TaskUserLogin", SqlHelper.ConnectionString, objparam);
                if (tblData.Rows.Count > 0)
                {
                    var result = new { tblData };
                    return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
                }
                else
                    return Utilities.GenerateApiResponse(true, (int)MessageType.error, "Invalid Username or Password", null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> ChangePassword(int UserId, string CurrentPassword, string NewPassword)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("UserId",UserId),
                    new SqlParameter("CurrentPassword",CurrentPassword),
                    new SqlParameter("NewPassword",NewPassword)
                };
                string message = string.Empty;
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_ChangePassword", SqlHelper.ConnectionString, objparam);
                if (tblData.Rows.Count > 0)
                    message = pub.GetString(tblData.Rows[0]["Result"]).Split("#")[1];
                return Utilities.GenerateApiResponse(true, pub.GetString(tblData.Rows[0]["Result"]).Split("#")[0].ToUpper() == "SUCCESS" ? (int)MessageType.success : (int)MessageType.error, message, null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetImageById(int UserId)
        {
            try
            {
                DataTable tblData = await SqlHelper.GetDataTable("SELECT UserId,ProfileImg,CONCAT(FirstName,' ',LastName) [Name] FROM Task_UserList WHERE UserId=" + UserId, SqlHelper.ConnectionString);
                var result = new { tblData };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetUserpermission(int UserId)
        {
            try
            {
                DataSet ds = await SqlHelper.GetDataSet("EXEC Task_GetUserpermission @UserId = " + UserId, SqlHelper.ConnectionString);
                var result = new { ds };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
    }
}
