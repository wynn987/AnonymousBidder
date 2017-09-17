﻿using System.Runtime.CompilerServices;
using AnonymousBidder.Common;

namespace AnonymousBidder.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using System.Data.Entity.ModelConfiguration;
    using AnonymousBidder.Data.Entity;

    public partial class AnonymousBidderDataContext : DbContext
    {
        public AnonymousBidderDataContext()
            : base("LocalAnonymousBidderDb") // change to below if we get our own server
        //: base(SqlConnectionHelper.GetEntityConnectionString())
        {
        }

        #region Entities
        public virtual DbSet<Auction> Auction { get; set; }
        public virtual DbSet<Bid> Bid { get; set; }
        public virtual DbSet<FilePath> FilePath { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        #endregion Entities


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where !string.IsNullOrEmpty(t.Namespace) &&
                                        t.BaseType != null
                                        && t.BaseType.BaseType != null
                                        && t.BaseType.BaseType.IsGenericType
                                  let genericType = t.BaseType.BaseType.GetGenericTypeDefinition()
                                  where genericType == typeof(EntityTypeConfiguration<>) || genericType == typeof(ComplexTypeConfiguration<>)
                                  select t;

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }
    }
}
