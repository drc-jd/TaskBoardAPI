using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskBoardAPI.Class
{
    public class LoginResponseData
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public string Token_type { get; set; }
        public string Client_id { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public byte[] profileImg { get; set; }
        public string role { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
    }
}
