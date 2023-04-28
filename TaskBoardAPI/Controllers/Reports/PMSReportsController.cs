using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TaskBoardAPI.Utils;

namespace TaskBoardAPI.Controllers.Reports
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PMSReportsController : ControllerBase
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
                    case "GETDATA":
                        response = await GetData(pub.GetString(paramList.ProType), pub.GetInt(paramList.UserId), pub.GetInt(paramList.ProjectId), Convert.ToDateTime(paramList.SDate).ToLocalTime(), Convert.ToDateTime(paramList.EDate).ToLocalTime());
                        break;
                    case "FILLCOMBO":
                        response = await FillCombo(pub.GetString(paramList.Role), pub.GetInt(paramList.UserId));
                        break;
                }
                return response;
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> FillCombo(string Role, int UserId)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type","FillCombo"),
                    new SqlParameter("Role",Role),
                    new SqlParameter("UserId",UserId)
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_PMSReport", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
                var result = new { ds };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
        private async Task<ApiResponse> GetData(string Type, int UserId, int ProjectId, DateTime SDate, DateTime EDate)
        {
            try
            {
                SqlParameter[] objparam = new SqlParameter[]
                {
                    new SqlParameter("Type",Type),
                    new SqlParameter("UserId",UserId),
                    new SqlParameter("ProjectId",ProjectId),
                    new SqlParameter("SDate",SDate.ToString("MM-dd-yyyy")),
                    new SqlParameter("EDate",EDate.ToString("MM-dd-yyyy")),
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_PMSReport", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
                DataTable tblData = ds.Tables[0];
                tblData.Columns.Add("Total");
                foreach (DataRow dr in tblData.Rows)
                {
                    decimal total = 0;
                    for (int i = 2; i < 9; i++)
                    {
                        total += pub.GetDecimal(dr[i]);
                        if ((total - Math.Floor(total)) > 0.59M)
                            total = (int)total + ((total - Math.Floor(total)) - 0.60M) + 1;
                    }
                    dr["Total"] = total;
                }
                DataTable tblDates = ds.Tables[1];
                DataTable tblProjects = ds.Tables[2];
                DataTable tblDetails = ds.Tables[3];
                var result = new { tblData, tblDates, tblProjects, tblDetails };
                return Utilities.GenerateApiResponse(true, (int)MessageType.success, "", result);
            }
            catch (Exception ex)
            {
                return Utilities.GenerateApiResponse(true, (int)MessageType.error, ex.Message, null);
            }
        }
    }
}
