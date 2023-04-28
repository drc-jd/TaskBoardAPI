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
    public class DatewiseTasksReportController : ControllerBase
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
                        response = await GetData(paramList);
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
                    new SqlParameter("SDate",Convert.ToDateTime(paramList.SDate).ToLocalTime().ToString("MM-dd-yyyy")),
                    new SqlParameter("EDate",Convert.ToDateTime(paramList.EDate).ToLocalTime().ToString("MM-dd-yyyy")),
                    new SqlParameter("Pending",pub.Getbool(paramList.Pending)),
                    new SqlParameter("Details",pub.Getbool(paramList.Details)),
                };
                DataSet ds = await SqlHelper.GetDataSet("Task_DatewiseTasksReport", SqlHelper.ConnectionString, CommandType.StoredProcedure, objparam);
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
