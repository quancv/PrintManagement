using PrintManagement.Domain.Enumerates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        
        public int? TeamId { get; set; }
        public virtual Team? Team { get; set; }
        
        public ConstantEnums.UserStatusEnum UserStatus { get; set; } = ConstantEnums.UserStatusEnum.UnActivated;
        public virtual ICollection<Permissions>? Permissions { get; set; }
        public virtual ICollection<Project>? Projects { get; set;}

    }

    
}
