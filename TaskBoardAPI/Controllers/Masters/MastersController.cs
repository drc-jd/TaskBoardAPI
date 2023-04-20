using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : ControllerBase
    {
        [HttpGet]
        [Route("Test")]
        public string Test()
        {
            return "API Is Working.";
        }
        [HttpPost]
        [Route("Data")]
        public async Task<ApiResponse> Data(dynamic paramList)
        {
            try
            {
                ApiResponse response = new ApiResponse();
                switch (pub.GetString(paramList.Type).ToUpper())
                {
                    case "PROJECTLISTCRUD":
                        response = await ProjectListCrud(paramList);
                        break;
                    case "FILLCOMBO":
                        response = await FillCombo();
                        break;
                    case "USERSCRUD":
                        response = await UsersCrud(paramList);
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> ProjectListCrud(dynamic paramList)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Mode",pub.GetString(paramList.Mode)),
                    new SqlParameter("SrNo",pub.GetInt(paramList.SrNo)),
                    new SqlParameter("ProjectName",pub.GetString(paramList.ProjectName)),
                    new SqlParameter("HeadPerson",pub.GetString(paramList.HeadPerson)),
                    new SqlParameter("Impact",pub.GetString(paramList.Impact)),
                    new SqlParameter("Description",pub.GetString(paramList.Description)),
                    new SqlParameter("Developers",pub.GetString(paramList.Developers)),
                    new SqlParameter("Incharge",pub.GetString(paramList.Incharge)),
                    new SqlParameter("Manager",pub.GetString(paramList.Manager)),
                    new SqlParameter("Auth",pub.GetString(paramList.Auth)),
                };
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectListCRUD", SqlHelper.ConnectionString, objparam);

                var result = new { tblData };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> FillCombo()
        {
            try
            {
                DataTable tblProject = await SqlHelper.GetDataTable("SELECT SrNo,ProjectName FROM Task_ProjectList", SqlHelper.ConnectionString);
                DataTable tblAuth = await SqlHelper.GetDataTable("SELECT UserId [value],CONCAT(FirstName,' ',LastName) [text] FROM Task_UserList where Role='Approval Authority'", SqlHelper.ConnectionString);
                DataTable tblDeveloper = await SqlHelper.GetDataTable("SELECT UserId [id],CONCAT(FirstName,' ',LastName) [itemName] FROM Task_UserList WHERE [Role]='Developer'", SqlHelper.ConnectionString);
                DataTable tblManager = await SqlHelper.GetDataTable("SELECT UserId [value],CONCAT(FirstName,' ',LastName) [text] FROM Task_UserList WHERE [Role]='Manager'", SqlHelper.ConnectionString);
                DataTable tblTaskIncharge = await SqlHelper.GetDataTable("SELECT UserId [id],CONCAT(FirstName,' ',LastName) [itemName] FROM Task_UserList WHERE [Role]='Task Incharge'", SqlHelper.ConnectionString);

                var result = new { tblProject, tblAuth, tblDeveloper, tblManager, tblTaskIncharge };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> UsersCrud(dynamic paramList)
        {
            try
            {
                byte[] profileImg = paramList.profileImg;

                List<SqlParameter> objparam = new List<SqlParameter>()
                {
                    new SqlParameter("Mode",pub.GetString(paramList.Mode)),
                    new SqlParameter("SrNo",pub.GetInt(paramList.SrNo)),
                    new SqlParameter("FirstName",pub.GetString(paramList.FirstName)),
                    new SqlParameter("LastName",pub.GetString(paramList.LastName)),
                    new SqlParameter("Email",pub.GetString(paramList.Email)),
                    new SqlParameter("UserName",pub.GetString(paramList.UserName)),
                    new SqlParameter("Password",pub.GetString(paramList.Password)),
                    new SqlParameter("MobileNo",pub.GetString(paramList.MobileNo)),
                    new SqlParameter("ExtNo",pub.GetString(paramList.ExtNo)),
                    new SqlParameter("Role",pub.GetString(paramList.Role)),
                    new SqlParameter("Projects",pub.GetString(paramList.Projects)),
                };
                SqlParameter imageParameter = new SqlParameter("ProfileImg", SqlDbType.Image);

                if (profileImg == null)
                    imageParameter.Value = DBNull.Value;
                else
                    imageParameter.Value = profileImg;

                objparam.Add(imageParameter);

                DataTable tblData = await SqlHelper.GetDatatableSP("Task_UserListCRUD", SqlHelper.ConnectionString, objparam.ToArray());

                var result = new { tblData };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
    }
}
