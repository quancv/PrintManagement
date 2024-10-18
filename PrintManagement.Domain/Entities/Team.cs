using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } 
        public string Description { get; set; } 
        public int NumberOfMember { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; }
        public int ManagerId { get; set; } 
        public virtual ICollection<User>? Members { get; set; }
    }
}
