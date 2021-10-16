using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Application.Contracts.Dtos.Organization.Email
{
    public class EmailInputDto
     {
        public int? FirstUserId { get; set; }
        public int SecondUserId { get; set; } // front end proeprty has userid
        public string FirstUserEmail { get; set; }
        public string SecondUserEmail{ get; set; }
        public string Message { get; set; }
    }
}
