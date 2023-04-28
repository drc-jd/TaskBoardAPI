using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TaskBoardAPI.Class;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
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
                    case "FILLCOMBO":
                        response = await FillCombo(pub.GetInt(paramList.ProjectId), pub.GetInt(paramList.UserId));
                        break;
                    case "GETPROJECTBYUSER":
                        response = await GetProjectByUser(pub.GetInt(paramList.UserId), pub.GetString(paramList.Role));
                        break;
                    case "GETPENDINGTASK":
                        response = await GetPendingTask(pub.GetInt(paramList.UserId), pub.GetInt(paramList.ProjectId), pub.GetString(paramList.Role), pub.GetInt(paramList.DevId));
                        break;
                    case "INSERTCOMMENT":
                        response = await InsertComment(pub.GetInt(paramList.RefSrNo), pub.GetInt(paramList.Hours), pub.GetInt(paramList.Minutes), pub.GetString(paramList.Comment));
                        break;
                    case "UPDATEPRIORITY":
                        List<DevComment> priority = JsonConvert.DeserializeObject<List<DevComment>>(paramList.priority.ToString()) as List<DevComment>;
                        response = await UpdatePriority(priority);
                        break;
                    case "UPDATEPRIORITYDATE":
                        response = await UpdatePriorityDate(pub.GetInt(paramList.TaskId));
                        break;
                    case "GETCOMMENTS":
                        response = await GetComments(pub.GetInt(paramList.RefSrNo));
                        break;
                    case "DELETECOMMENT":
                        response = await DeleteComment(pub.GetInt(paramList.SrNo));
                        break;
                    case "COMPLETECOMMENT":
                        response = await CompleteComment(pub.GetInt(paramList.SrNo));
                        break;
                    case "COMPLETETASK":
                        response = await CompleteTask(pub.GetInt(paramList.SrNo));
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> FillCombo(int ProjectId, int UserId)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","FillCombo"),
                    new SqlParameter("UserId",UserId),
                    new SqlParameter("ProjectId",ProjectId)
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_ProjectsSP", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);

                var result = new { ds };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetProjectByUser(int UserId, string Role)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetProjectByUser"),
                    new SqlParameter("UserId",UserId),
                    new SqlParameter("Role",Role)
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_ProjectsSP", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);

                var result = new { ds };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetPendingTask(int UserId, int ProjectId, string Role, int DevId)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetPendingTask"),
                    new SqlParameter("UserId",UserId),
                    new SqlParameter("ProjectId",ProjectId),
                    new SqlParameter("Role",Role),
                    new SqlParameter("DevId",DevId)
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_ProjectsSP", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);

                DataTable tblDuration = new DataTable();
                tblDuration.Columns.Add("TaskID");
                tblDuration.Columns.Add("Hours");
                tblDuration.Columns.Add("Minutes");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataTable tblData = await SqlHelper.GetDataTable("EXEC Task_GetDurationByTaskId @TaskId=" + dr["id"], SqlHelper.ConnectionString);
                    if (tblData.Rows.Count > 0)
                        tblDuration.Rows.Add(pub.GetInt(tblData.Rows[0]["TaskId"]), pub.GetInt(tblData.Rows[0]["Hours"]), pub.GetInt(tblData.Rows[0]["Minutes"]));
                }

                var result = new { ds, tblDuration };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetComments(int RefSrNo)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetComments"),
                    new SqlParameter("RefSrNo",RefSrNo)
                };

                DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam);
                var result = new { tblData };

                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> InsertComment(int RefSrNo, int Hours, int Minutes, string Comment)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","InsertComment"),
                    new SqlParameter("RefSrNo",RefSrNo),
                    new SqlParameter("Hours",Hours),
                    new SqlParameter("Minutes",Minutes),
                    new SqlParameter("Comment",Comment)
                };

                string message = string.Empty;
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam);
                message = pub.GetString(tblData.Rows[0]["Result"]);
                if (message.ToUpper() == "SUCCESS")
                {
                    string srNo = pub.GetString(tblData.Rows[0]["SrNo"]);
                    var result = new { srNo };
                    return Utilities.GenerateApiResponse(true, (int)MessageType.success, message, result);
                }
                else
                    return Utilities.GenerateApiResponse(true, (int)MessageType.error, message, null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> UpdatePriority(List<DevComment> priority)
        {
            try
            {
                DataTable tblType = Utilities.ToDataTable(priority);

                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","UpdatePriority"),
                    new SqlParameter("tblPriority",tblType),
                };

                await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam);
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> UpdatePriorityDate(int TaskId)
        {
            try
            {
                await SqlHelper.ExecuteNonQuery("UPDATE Task_PMSTask SET PriorityDate=GETDATE() WHERE TaskID=" + TaskId, SqlHelper.ConnectionString);
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> DeleteComment(int SrNo)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","DeleteComment"),
                    new SqlParameter("SrNo",SrNo)
                };

                string message = string.Empty;
                using (DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam))
                    message = pub.GetString(tblData.Rows[0]["Result"]);

                return Utilities.GenerateApiResponse(true, message.ToUpper() == "SUCCESS" ? (int)MessageType.success : (int)MessageType.error, message, null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> CompleteComment(int SrNo)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","CompleteComment"),
                    new SqlParameter("SrNo",SrNo)
                };

                string message = string.Empty;
                using (DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam))
                    message = pub.GetString(tblData.Rows[0]["Result"]);

                return Utilities.GenerateApiResponse(true, message.ToUpper() == "SUCCESS" ? (int)MessageType.success : (int)MessageType.error, message, null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> CompleteTask(int SrNo)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","CompleteTask"),
                    new SqlParameter("SrNo",SrNo)
                };

                string message = string.Empty;
                using (DataTable tblData = await SqlHelper.GetDatatableSP("Task_ProjectsSP", SqlHelper.ConnectionString, objparam))
                    message = pub.GetString(tblData.Rows[0]["Result"]);

                return Utilities.GenerateApiResponse(true, message.ToUpper() == "SUCCESS" ? (int)MessageType.success : (int)MessageType.error, message, null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
    }
}
