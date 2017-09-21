using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class UserRoleModel
    {
        public Guid UserRoleGUID { get; set; }
        public string UserRoleName { get; set; }

        public virtual ICollection<ABUserModel> ABUsers { get; set; }

        public UserRoleModel()
        {
            ABUsers = new List<ABUserModel>();
        }
    }
}