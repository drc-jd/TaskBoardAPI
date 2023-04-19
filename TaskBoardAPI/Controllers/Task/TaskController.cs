using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskBoardAPI.Class;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Task
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
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
                    case "FILLCOMBO":
                        response = await FillCombo(pub.GetString(paramList.ProType).ToUpper(), pub.GetInt(paramList.UserId), pub.GetInt(paramList.ProjectId));
                        break;
                    case "ADDBYINCHARGE":
                        response = await AddByIncharge(pub.GetString(paramList.spType), pub.GetInt(paramList.taskIncharge), pub.GetString(paramList.taskName), pub.GetInt(paramList.projectId), pub.GetString(paramList.taskDescription), pub.GetInt(paramList.developerId), pub.GetInt(paramList.ipriorityId));
                        break;
                    case "GETTASKLIST":
                        response = await GetTaskList(pub.GetString(paramList.Role), pub.GetInt(paramList.UserId), pub.GetString(paramList.ReportType));
                        break;
                    case "RECEIVETASKBYDEV":
                        response = await ReceiveTaskByDev(paramList);
                        break;
                    case "APPROVEBYAUTH":
                        response = await ApproveByAuth(pub.GetInt(paramList.TaskId), pub.GetString(paramList.AComment));
                        break;
                    case "SUBMITRATING":
                        response = await SubmitRating(pub.GetInt(paramList.TaskId), pub.GetInt(paramList.Rating));
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> FillCombo(string ProType, int UserId, int ProjectId)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type",pub.GetString(ProType)),
                    new SqlParameter("UserId",pub.GetInt(UserId)),
                    new SqlParameter("ProjectId",pub.GetInt(ProjectId)),
                };
                DataTable tblData = await SqlHelper.GetDatatableSP("usp_TaskGetProjectByUserId", SqlHelper.ConnectionString, objparam);

                var result = new { tblData };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> AddByIncharge(string spType, int taskIncharge, string taskName, int projectId, string taskDescription, int developerId, int ipriorityId)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type",spType),
                    new SqlParameter("TaskInchargeID",taskIncharge),
                    new SqlParameter("TaskName",taskName),
                    new SqlParameter("ProjectID",projectId),
                    new SqlParameter("TaskDescription",taskDescription),
                    new SqlParameter("DeveloperID",developerId),
                    new SqlParameter("IPriorityID",ipriorityId)
                };
                string message = string.Empty;
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_PMSTaskActions", SqlHelper.ConnectionString, objparam);
                message = pub.GetString(tblData.Rows[0]["Result"]);
                if (message.ToUpper() == "SUCCESS")
                {
                    string taskId = pub.GetString(tblData.Rows[0]["TaskId"]);
                    var result = new { taskId };
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
        private async Task<ApiResponse> ReceiveTaskByDev(dynamic paramList)
        {
            try
            {
                List<DevComment> comments = JsonConvert.DeserializeObject<List<DevComment>>(paramList.DevComment.ToString()) as List<DevComment>;
                int TaskId = pub.GetInt(paramList.TaskId);
                DateTime TaskStartDate = Convert.ToDateTime(paramList.TaskStartDate).ToLocalTime();
                int TotalHours = pub.GetInt(paramList.TotalHours);
                int TotalMinutes = pub.GetInt(paramList.TotalMinutes);
                string TaskType = pub.GetString(paramList.TaskType);

                DataTable tblType = Utilities.ToDataTable(comments);

                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("TaskId",TaskId),
                    new SqlParameter("TaskStartDate",TaskStartDate.ToLocalTime().ToString("MM/dd/yyyy")),
                    new SqlParameter("TotalHours",TotalHours),
                    new SqlParameter("TotalMinutes",TotalMinutes),
                    new SqlParameter("TaskType",TaskType),
                    new SqlParameter("tblType",tblType)
                };
                string message = string.Empty;
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_ReceiveTaskByDev", SqlHelper.ConnectionString, objparam);
                if (pub.GetString(tblData.Rows[0]["Result"]).ToUpper() == "SUCCESS")
                    return Utilities.GenerateApiResponse(true, (int)MessageType.success, "Task Received Successfully", null);
                else
                    return Utilities.GenerateApiResponse(true, (int)MessageType.error, pub.GetString(tblData.Rows[0]["Result"]), null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> ApproveByAuth(int TaskId, string AComment)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","ApproveByAuth"),
                    new SqlParameter("TaskId",TaskId),
                    new SqlParameter("AComment",AComment)
                };
                DataTable tblData = await SqlHelper.GetDatatableSP("Task_PMSTaskActions", SqlHelper.ConnectionString, objparam);
                if (pub.GetString(tblData.Rows[0]["Result"]).ToUpper() == "SUCCESS")
                    return Utilities.GenerateApiResponse(true, (int)MessageType.success, "Successfully Approved", null);
                else
                    return Utilities.GenerateApiResponse(true, (int)MessageType.error, pub.GetString(tblData.Rows[0]["Result"]), null);

            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetTaskList(string Role, int UserId, string ReportType)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","GetTaskList"),
                    new SqlParameter("Role",Role),
                    new SqlParameter("ReportType",ReportType),
                    new SqlParameter("UserId",UserId)
                };
                string message = string.Empty;
                DataSet ds = await SqlHelper.GetDataSet("Task_PMSTaskActions", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
                DataTable tblData = ds.Tables[0];
                DataTable tblDevComments = new DataTable();
                if (Role.ToUpper() != "DEVELOPER")
                {
                    if (ds.Tables.Count > 1)
                        tblDevComments = ds.Tables[1];
                }
                DataTable tblDuration = new DataTable();
                tblDuration.Columns.Add("TaskID");
                tblDuration.Columns.Add("Hours");
                tblDuration.Columns.Add("Minutes");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataTable dt = await SqlHelper.GetDataTable("EXEC Task_GetDurationByTaskId @TaskId=" + dr["id"], SqlHelper.ConnectionString);
                    if (dt.Rows.Count > 0)
                        tblDuration.Rows.Add(pub.GetInt(dt.Rows[0]["TaskId"]), pub.GetInt(dt.Rows[0]["Hours"]), pub.GetInt(dt.Rows[0]["Minutes"]));
                }
                var result = new { tblData, tblDevComments, tblDuration };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, message, result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> SubmitRating(int TaskId, int Rating)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","SubmitRating"),
                    new SqlParameter("Rating",Rating),
                    new SqlParameter("TaskId",TaskId)
                };
                string message = string.Empty;

                DataTable tblData = await SqlHelper.GetDatatableSP("Task_PMSTaskActions", SqlHelper.ConnectionString, objparam);
                return Utilities.GenerateApiResponse(true, (pub.GetString(tblData.Rows[0]["Result"]).ToUpper() == "SUCCESS" ? (int)MessageType.success : (int)MessageType.error), pub.GetString(tblData.Rows[0]["Result"]), null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        [HttpPost]
        [Route("Upload/{TaskId}/{Role}/{CSrNo}")]
        public async Task<ApiResponse> UploadFile(List<IFormFile> files, string TaskId, string Role, int CSrNo)
        {
            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var baseDrive = SqlHelper.baseDrive + (CSrNo == 0 ? "\\Task Incharge" : "\\Developer") ;

                    if (!Directory.Exists(baseDrive))
                        Directory.CreateDirectory(baseDrive);
                    int id = (CSrNo == 0 ? pub.GetInt(TaskId) : CSrNo);
                    var fileName = files[i].FileName.Split(".")[0] + "_" + pub.GetString(id) + "_" + (i + 1) + "." + files[i].FileName.Split(".")[files[i].FileName.Split(".").Length - 1];
                    var filePath = Path.Combine(baseDrive, fileName.ToString());

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await files[i].CopyToAsync(fs).ConfigureAwait(false);
                    }
                    SqlParameter[] objparam = new SqlParameter[]
                    {
                        new SqlParameter("Type","FileSave"),
                        new SqlParameter("TaskId",pub.GetInt(TaskId)),
                        new SqlParameter("CSrNo",CSrNo),
                        new SqlParameter("Role",Role),
                        new SqlParameter("FileName",fileName)
                    };
                    DataTable tblData = await SqlHelper.GetDatatableSP("Task_PMSTaskActions", SqlHelper.ConnectionString, objparam);
                }
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, (files.Count) + " File(s) Uploaded", null);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        [HttpGet]
        [Route("Download/{FileName}")]
        public async Task<IActionResult> Download(string FileName)
        {
            try
            {

                //var dataStream = new MemoryStream();
                //using (var fs = new FileStream(SqlHelper.baseDrive + "\\Task Incharge\\" + FileName, FileMode.Open))
                //{
                //    await dataStream.CopyToAsync(fs).ConfigureAwait(false);
                //}

                //return File(dataStream, "application/octet-stream", FileName);

                byte[] bytes = System.IO.File.ReadAllBytes(SqlHelper.baseDrive + "\\Task Incharge\\" + FileName);
                return File(bytes, "application/octet-stream", FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
