using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WorkerService.Dto
{
    public class EmailDto
    {
        public int FirstUserId { get; set; }
        public int SecondUserId { get; set; }
        public string Message { get; set; }
    }
}
