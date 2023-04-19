using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TaskBoardAPI.Utils
{
    public class Utilities
    {

        public static ApiResponse GenerateApiResponse(bool IsValidUser, int MessageType, string Message, object DataList)
        {
            ApiResponse apiResponse = new ApiResponse();
            apiResponse.IsValidUser = IsValidUser;
            apiResponse.MessageType = MessageType;
            apiResponse.Message = Message.ToString();
            apiResponse.DataList = DataList;
            return apiResponse;
        }
        public static DataTable RenameColumn(DataTable tblSource, ColumnCaption[] lstColumn)
        {
            for (int i = 0; i < tblSource.Columns.Count; i++)
            {
                foreach (ColumnCaption Col in lstColumn)
                {
                    if (tblSource.Columns[i].ColumnName.ToUpper() == Col.OldCaption.ToUpper())
                    {
                        tblSource.Columns[i].ColumnName = Col.NewCaption;
                    }
                }
            }
            return tblSource;
        }
        public static DataTable ToDataTable<T>(ICollection<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
    public class ColumnCaption
    {
        public string OldCaption { get; set; }
        public string NewCaption { get; set; }
    }


}
