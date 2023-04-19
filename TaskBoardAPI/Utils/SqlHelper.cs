using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TaskBoardAPI.Utils
{
    public class SqlHelper
    {
        public static string ConnectionString { get; set; }
        public static string baseDrive { get; set; }
        public static async Task<DataTable> GetDatatableSP(string spname, string con, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(con))
                {
                    if (Conn.State == ConnectionState.Closed)
                        Conn.Open();

                    using (SqlCommand command = new SqlCommand(spname, Conn))
                    {
                        command.CommandTimeout = 0;
                        command.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                            foreach (SqlParameter p in parameters)
                                if (p != null) command.Parameters.Add(p);

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            using (DataTable tb = new DataTable())
                            {
                                await Task.Run(() => tb.Load(dr));
                                return tb;
                            }
                        }
                    }

                }
            }
            catch (SqlException se)
            {
                throw se;
            }
        }
        public static async Task<DataTable> GetDataTable(string query, string connString, CommandType commandType = CommandType.Text, SqlParameter[] sqlParameterCollection = null)
        {
            DataTable dt = null;
            try
            {
                using (SqlConnection Conn = new SqlConnection(connString))
                {
                    try
                    {
                        Conn.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(query, Conn);
                        dt = new DataTable();
                        sda.SelectCommand.CommandTimeout = 0;
                        sda.SelectCommand.CommandType = commandType;
                        if (sqlParameterCollection != null && sqlParameterCollection.Length > 0)
                            sda.SelectCommand.Parameters.AddRange(sqlParameterCollection);
                        await Task.Run(() => sda.Fill(dt));
                    }
                    catch (SqlException se)
                    {
                        throw se;
                    }
                    finally
                    {
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                throw ex;
            }

            return dt;
        }
        public static async Task<DataRow> GetDataRow(string query, string connString, CommandType commandType = CommandType.Text, SqlParameter[] sqlParameterCollection = null)
        {
            DataRow dr = null;
            try
            {
                using (SqlConnection Conn = new SqlConnection(connString))
                {
                    try
                    {
                        Conn.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(query, Conn);
                        sda.SelectCommand.CommandTimeout = 0;
                        sda.SelectCommand.CommandType = commandType;
                        if (sqlParameterCollection != null && sqlParameterCollection.Length > 0)
                            sda.SelectCommand.Parameters.AddRange(sqlParameterCollection);
                        DataTable dt = new DataTable();
                        await Task.Run(() => sda.Fill(dt));
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dr = dt.Rows[0];
                        }
                    }
                    catch (SqlException se)
                    {
                        throw se;
                    }
                    finally
                    {
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                dr = null;
                throw ex;
            }

            return dr;
        }
        public static async Task<DataSet> GetDataSet(string query, string connString, CommandType commandType = CommandType.Text, SqlParameter[] sqlParameterCollection = null)
        {
            DataSet ds = null;
            try
            {
                using (SqlConnection Conn = new SqlConnection(connString))
                {
                    try
                    {
                        Conn.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(query, Conn);
                        sda.SelectCommand.CommandTimeout = 0;
                        sda.SelectCommand.CommandType = commandType;
                        if (sqlParameterCollection != null && sqlParameterCollection.Length > 0)
                            sda.SelectCommand.Parameters.AddRange(sqlParameterCollection);
                        ds = new DataSet();
                        await Task.Run(() => sda.Fill(ds));
                    }
                    catch (SqlException se)
                    {
                        throw se;
                    }
                    finally
                    {
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }

            return ds;
        }
        public static async Task<Int32> ExecuteNonQuery(String query, string connString)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(connString))
                {
                    try
                    {
                        int execute;
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = Conn;
                            command.CommandTimeout = 0;
                            command.CommandText = query;
                            Conn.Open();
                            execute = await command.ExecuteNonQueryAsync();
                            Conn.Close();
                        }
                        return execute;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
