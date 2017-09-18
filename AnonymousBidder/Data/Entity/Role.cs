using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class UserRole : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserRoleGUID { get; set; }
        [Required]
        public string UserRoleName { get; set; }

        public virtual ICollection<ABUser> ABUsers { get; set; }

        public UserRole()
        {
            ABUsers = new List<ABUser>();
        }
    }
}