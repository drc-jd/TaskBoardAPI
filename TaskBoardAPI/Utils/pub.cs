using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskBoardAPI.Utils
{
    public class pub
    {
        public static double GetDouble(object theValue)
        {
            try
            {
                if (theValue == DBNull.Value || theValue == string.Empty)
                {
                    return 0;
                }
                return Convert.ToDouble(theValue);
            }
            catch
            {
                return 0;
            }
        }

        public static decimal GetDecimal(object theValue)
        {
            try
            {
                if (theValue == DBNull.Value || theValue == string.Empty)
                {
                    return 0;
                }
                return Convert.ToDecimal(theValue);
            }
            catch
            {
                return 0;
            }
        }
        public static Int32 GetInt(object theValue)
        {
            try
            {
                if (theValue == "" || theValue == string.Empty || theValue == DBNull.Value)
                {
                    return 0;
                }
                return Convert.ToInt32(theValue);
            }
            catch
            {
                return 0;
            }
        }
        public static Boolean Getbool(object theValue)
        {
            try
            {
                if (theValue == "" || theValue == string.Empty || theValue == DBNull.Value)
                {
                    return false;
                }
                return Convert.ToBoolean(theValue);
            }
            catch
            {
                return false;
            }
        }
        public static string GetString(object theValue)
        {
            try
            {
                if (theValue == DBNull.Value || theValue == string.Empty)
                {
                    return "";
                }
                return Convert.ToString(theValue);
            }
            catch
            {
                return "";
            }
        }
    }
    public enum MessageType
    {
        login = 0,
        success = 1,
        error = 2
    }
    public enum RoleType
    {
        ApprovalAuthority = 0,
        TaskIncharge = 1,
        Developer = 2
    }
}
