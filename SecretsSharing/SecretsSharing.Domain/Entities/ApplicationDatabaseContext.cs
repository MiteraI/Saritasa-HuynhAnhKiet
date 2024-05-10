using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Domain.Entities
{
    public class ApplicationDatabaseContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
        {
        }

        public DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .IsRequired();

                entity.ToTable("user");
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
            });

            builder.Entity<Upload>(entity =>
            {
                entity.Property(e => e.UploadType)
                    .HasConversion<string>();

                entity.ToTable("upload");
            });
        }
    }
}
