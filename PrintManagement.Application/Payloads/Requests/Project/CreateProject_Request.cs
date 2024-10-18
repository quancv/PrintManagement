using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrintManagement.Domain.Enumerates.ConstantEnums;

namespace PrintManagement.Application.Payloads.Requests.Project
{
    public class CreateProject_Request
    {
        public string ProjectName { get; set; } = string.Empty;
        public string RequestDescriptionFromCustomer { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Image { get; set; } = string.Empty;
        public DateTime ExpectedEndDate { get; set; }
        public ProjectStatus ProjectStatus { get; set; } 

        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
    }
}
