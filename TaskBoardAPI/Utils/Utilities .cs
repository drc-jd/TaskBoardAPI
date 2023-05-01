using System;
using System.Collections.Generic;
using System.Data;
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
        public static string GetMd5HashWithMySecurityAlgo(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.  
            return sBuilder.ToString();
        }
        public static bool VerifyMd5HashWithMySecurityAlgo(string input, string hash)
        {
            // Hash the input.  
            string hashOfInput = GetMd5HashWithMySecurityAlgo(input);
            // Create a StringComparer an compare the hashes.  
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class ColumnCaption
    {
        public string OldCaption { get; set; }
        public string NewCaption { get; set; }
    }


}
