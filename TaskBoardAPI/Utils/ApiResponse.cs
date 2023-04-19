using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskBoardAPI.Utils
{
    public class ApiResponse
    {
        public bool IsValidUser { get; set; }
        public int MessageType { get; set; } //0:error, 1: success, 2: warning, 3: information 
        public string Message { get; set; }
        public object DataList { get; set; }
    }
}
