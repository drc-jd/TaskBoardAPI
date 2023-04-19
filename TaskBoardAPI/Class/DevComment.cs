using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskBoardAPI.Class
{
    public class DevComment
    {
        public int TaskId { get; set; }
        public int SeqNo { get; set; }
        public int DeveloperId { get; set; }
        public int ApproxHours { get; set; }
        public int ApproxMinutes { get; set; }
        public string Comment { get; set; }
    }
}
