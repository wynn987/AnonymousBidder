﻿using System;
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
        public Guid RoleGUID { get; set; }
        [Required]
        public string RoleName { get; set; }

        public virtual ICollection<ABUser> Users { get; set; }

        public UserRole()
        {
            Users = new List<User>();
        }
    }
}