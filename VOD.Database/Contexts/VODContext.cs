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

            if (!Courses.Any())
            {
                var instructorId1 = Instructors.First().Id;
                var instructorId2 = Instructors.Skip(1).FirstOrDefault().Id;

                var courses = new List<Course>
                {
                    new Course
                    {
                        InstructorId = instructorId1,
                        Title = "Course 1",
                        Description = description,
                        ImageUrl = "/images/course1.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    },
                    new Course
                    {
                        InstructorId = instructorId2,
                        Title = "Course 2",
                        Description = description,
                        ImageUrl = "/images/course2.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    },
                    new Course
                    {
                        InstructorId = instructorId1,
                        Title = "Course 3",
                        Description = description,
                        ImageUrl = "/images/course3.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    }
                };
                Courses.AddRange(courses);
                SaveChanges();
            }

            if (Courses.Count() < 3) return;

            var courseId1 = Courses.First().Id;
            var courseId2 = Courses.Skip(1).FirstOrDefault().Id;
            var courseId3 = Courses.Skip(2).FirstOrDefault().Id;

            if (!UserCourses.Any())
            {
                if (!courseId1.Equals(int.MinValue))
                    UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId1 });

                if (!courseId2.Equals(int.MinValue))
                    UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId2 });

                if (!courseId3.Equals(int.MinValue))
                    UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId3 });

                SaveChanges();
            }

            if (UserCourses.Count() < 3) return;

            if (!Modules.Any())
            {
                var modules = new List<Module>
                {
                    new Module { Course = Find<Course>(courseId1), Title = "Module 1" },
                    new Module { Course = Find<Course>(courseId1), Title = "Module 2" },
                    new Module { Course = Find<Course>(courseId2), Title = "Module 3" },
                };
                Modules.AddRange(modules);
                SaveChanges();
            }

            if (Modules.Count() < 3) return;

            var moduleId1 = Modules.First().Id;
            var moduleId2 = Modules.Skip(1).FirstOrDefault().Id;
            var moduleId3 = Modules.Skip(2).FirstOrDefault().Id;

            if (!Videos.Any())
            {
                var videos = new List<Video>
                {
                    new Video { ModuleId = moduleId1, CourseId = courseId1,
                    Title = "Video 1 Title",
                    Description = description.Substring(1,35),
                    Duration = 50, Thumbnail = "/images/video1.jpg",
                    Url =  "https://www.youtube.com/watch?v=BJFyzpBcaCY"
                    },
                    new Video { ModuleId = moduleId1, CourseId = courseId1,
                    Title = "Video 2 Title",
                    Description = description.Substring(5,35),
                    Duration = 45, Thumbnail = "/images/video2.jpg",
                    Url =  "https://www.youtube.com/watch?v=BJFyzpBcaCY"
                    },
                    new Video { ModuleId = moduleId1, CourseId = courseId1,
                    Title = "Video 3 Title",
                    Description = description.Substring(10,35),
                    Duration = 41, Thumbnail = "/images/video3.jpg",
                    Url =  "https://www.youtube.com/watch?v=BJFyzpBcaCY"
                    },
                    new Video { ModuleId = moduleId3, CourseId = courseId2,
                    Title = "Video 4 Title",
                    Description = description.Substring(15,35),
                    Duration = 41, Thumbnail = "/images/video4.jpg",
                    Url =  "https://www.youtube.com/watch?v=BJFyzpBcaCY"
                    },
                    new Video { ModuleId = moduleId2, CourseId = courseId1,
                    Title = "Video 5 Title",
                    Description = description.Substring(20,35),
                    Duration = 42, Thumbnail = "/images/video5.jpg",
                    Url =  "https://www.youtube.com/watch?v=BJFyzpBcaCY"
                    },
                };
                Videos.AddRange(videos);
                SaveChanges();
            }

            if (!Downloads.Any())
            {
                var downloads = new List<Download>
                {
                    new Download { ModuleId = moduleId1, CourseId = courseId1,
                    Title = "ADO.NET 1 (PDF)", Url = "https://some-url" },
                    new Download { ModuleId = moduleId1, CourseId = courseId1,
                    Title = "ADO.NET 2 (PDF)", Url = "https://some-url" },
                    new Download { ModuleId = moduleId3, CourseId = courseId2,
                    Title = "ADO.NET 1 (PDF)", Url = "https://some-url" },
                };
                Downloads.AddRange(downloads);
                SaveChanges();
            }
        }
    }
}
