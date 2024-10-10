using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Enumerates
{
    public class ConstantEnums
    {
        public enum UserStatusEnum 
        {
            UnActivated = 0,
            Activated = 1
        }
        public enum Role
        {
            Leader = 1,
            Designer = 2,
            Deliver = 3,
            Manager = 4,
            Employee = 5,
            Admin = 6
        }
    }
}
