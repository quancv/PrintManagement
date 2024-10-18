using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Requests.Teams
{
    public class CreateTeam_Request
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ManagerId { get; set; }
    }
}
