using Microsoft.AspNetCore.Authorization;
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
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [Route("Test")]
        public async Task<string> Test()
        {
            return "Testing Authorization..!";
        }

        [Authorize]
        [HttpPost]
        [Route("Data")]
        public async Task<ApiResponse> Data(dynamic paramList)
        {
            try
            {
                ApiResponse response = new ApiResponse();
                switch (pub.GetString(paramList.Type).ToUpper())
                {
                    case "GETDATA":
                        response = await GetData(paramList);
                        break;
                    case "GETTASK":
                        response = await GetTask(paramList);
                        break;
                    case "REOPENTASK":
                        response = await ReOpenTask(paramList);
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetData(dynamic paramList)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetData"),
                    new SqlParameter("Role",pub.GetString(paramList.Role)),
                    new SqlParameter("UserId",pub.GetInt(paramList.UserId)),
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_Dashboard", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
                var result = new { ds };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> ReOpenTask(dynamic paramList)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","ReOpenTask"),
                    new SqlParameter("SrNo",pub.GetInt(paramList.SrNo)),
                };
                string message = string.Empty;
                DataTable dt = await SqlHelper.GetDatatableSP("Task_Dashboard", SqlHelper.ConnectionString, objparam);
                if (dt.Rows[0]["Result"].ToString().ToUpper() == "SUCCESS")
                    return Utilities.GenerateApiResponse(true, (int)MessageType.success, "Task successfully Re-Opened..!", null);
                else
                    return Utilities.GenerateApiResponse(true, (int)MessageType.error, dt.Rows[0]["Result"].ToString(), null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetTask(dynamic paramList)
        {
            try
            {
                string ReportType = paramList.ReportType;
                int UserId = paramList.UserId;
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetTask"),
                    new SqlParameter("ReportType",ReportType),
                    new SqlParameter("UserId",UserId),
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_Dashboard", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
                DataTable tblDuration = new DataTable();
                tblDuration.Columns.Add("TaskID");
                tblDuration.Columns.Add("Hours");
                tblDuration.Columns.Add("Minutes");
                tblDuration.Columns.Add("TotalMins");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataTable tblData = await SqlHelper.GetDataTable("EXEC Task_GetDurationByTaskId @TaskId=" + dr["id"], SqlHelper.ConnectionString);
                    if (tblData.Rows.Count > 0)
                        tblDuration.Rows.Add(pub.GetInt(tblData.Rows[0]["TaskId"]), pub.GetInt(tblData.Rows[0]["Hours"]), pub.GetInt(tblData.Rows[0]["Minutes"]), pub.GetInt(tblData.Rows[0]["TotalMins"]));
                }
                var result = new { ds, tblDuration };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
    }
}
