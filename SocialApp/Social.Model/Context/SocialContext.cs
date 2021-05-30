using Microsoft.EntityFrameworkCore;
using Social.Core.Entity;
using Social.Core.Entity.Enums;
using Social.Model.Entities;
using Social.Model.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Model.Context
{
    public class SocialContext : DbContext
    {
        public SocialContext(DbContextOptions<SocialContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new ImageMap());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Following> Followings { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            var modifiedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added).ToList();

            DateTime date = DateTime.Now;

            foreach (var item in modifiedEntities)
            {
                CoreEntity entity = item.Entity as CoreEntity;

                if (entity != null)
                {
                    switch (item.State)
                    {
                        case EntityState.Modified:
                            entity.UpdatedDate = date;
                            break;
                        case EntityState.Added:
                            entity.CreatedDate = date;
                            entity.Status = Status.Active;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }



    }
}
