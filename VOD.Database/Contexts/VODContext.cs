using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VOD.Common.Entities;
using System.Linq;

namespace VOD.Database.Contexts
{
    public class VODContext : IdentityDbContext<VODUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Video> Videos { get; set; }

        public VODContext(DbContextOptions<VODContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedData(builder);
            builder.Entity<UserCourse>().HasKey(uc => new { uc.UserId, uc.CourseId });
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private void SeedData(ModelBuilder builder)
        {
            var email = "a@b.c";
            var password = "Test123_";

            var user = new VODUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true
            };

            var passwordHasher = new PasswordHasher<VODUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            builder.Entity<VODUser>().HasData(user);

            var admin = "Admin";
            var role = new IdentityRole
            {
                Id = "1",
                Name = admin,
                NormalizedName = admin.ToUpper()
            };

            builder.Entity<IdentityRole>().HasData(role);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = user.Id
            });

            builder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>
            {
                Id = 1,
                ClaimType = admin,
                ClaimValue = "true",
                UserId = user.Id
            });

            builder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>
            {
                Id = 2,
                ClaimType = "VODUser",
                ClaimValue = "true",
                UserId = user.Id
            });
        }

        public void SeedAdminData()
        {
            var adminEmail = "a@b.c";
            var adminPassword = "Test123_";
            var adminUserId = string.Empty;

            if (Users.Any(u => u.Email.Equals(adminEmail)))
                adminUserId = (Users.SingleOrDefault(u => u.Email.Equals(adminEmail))).Id;
            else
            {
                var user = new VODUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    NormalizedUserName = adminEmail.ToUpper()
                };
                var passwordHasher = new PasswordHasher<VODUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, adminPassword);
                Users.Add(user);
                SaveChanges();
                adminUserId = (Users.SingleOrDefault(u => u.Email.Equals(adminEmail))).Id;
            }

            var adminRoleName = "Admin";
            var adminRole = Roles.SingleOrDefault(r => r.Name.ToLower().Equals(adminRoleName.ToLower()));

            if (adminRole == default)
            {
                Roles.Add(new IdentityRole()
                {
                    Name = adminRoleName,
                    NormalizedName = adminRoleName.ToUpper(),
                    Id = "1"
                });

                SaveChanges();
                adminRole = Roles.SingleOrDefault(r => r.Name.ToLower().Equals(adminRoleName.ToLower()));
            }

            if (!adminUserId.Equals(string.Empty))
            {
                if (adminRole != default)
                {
                    var userRoleExists = UserRoles.Any(ur => ur.RoleId.Equals(adminRole.Id) && ur.UserId.Equals(adminUserId));

                    if (!userRoleExists)
                        UserRoles.Add(new IdentityUserRole<string>
                        { RoleId = adminRole.Id, UserId = adminUserId });
                }

                var claimType = "Admin";
                var userClaimExists = UserClaims.Any(uc => uc.ClaimType.ToLower().Equals(claimType.ToLower()) && uc.UserId.Equals(adminUserId));

                if (!userClaimExists)
                    UserClaims.Add(new IdentityUserClaim<string>
                    { ClaimType = claimType, ClaimValue = "true", UserId = adminUserId });

                claimType = "VODUser";
                userClaimExists = UserClaims.Any(uc => uc.ClaimType.ToLower().Equals(claimType.ToLower()) && uc.UserId.Equals(adminUserId));

                if (!userClaimExists)
                    UserClaims.Add(new IdentityUserClaim<string>
                    { ClaimType = claimType, ClaimValue = "true", UserId = adminUserId });
            }
            SaveChanges();
        }

        public void SeedMembershipData()
        {
            var description = "Overflow error ip fopen lib stack syn cache stack trace firewall infinite loop d00dz for chown throw race condition int bit ascii. Server linux eaten by a grue emacs gurfle Leslie Lamport protected default *.* pwned. Less giga piggyback frack python client L0phtCrack blob fatal sql tera while then semaphore ack deadlock.";
            var email = "a@b.c";
            var userId = string.Empty;

            if (Users.Any(r => r.Email.Equals(email)))
                userId = Users.First(r => r.Email.Equals(email)).Id;
            else
                return;

            if (!Instructors.Any())
            {
                var instructors = new List<Instructor>
                {
                    new Instructor
                    {
                        Name = "John Doe",
                        Description = description.Substring(20, 50),
                        Thumbnail = "/images/Ice-Age-Scrat-icon.png"
                    },
                    new Instructor
                    {
                        Name = "Jane Doe",
                        Description = description.Substring(30, 40),
                        Thumbnail = "/images/Ice-Age-Scrat-icon.png"
                    }
                };
                Instructors.AddRange(instructors);
                SaveChanges();
            }

            if (Instructors.Count() < 2) return;
        }
    }
}
