﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class Permissions : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public int RoleId { get; set; }
        public virtual Role? Role { get; set; }
    }
}
