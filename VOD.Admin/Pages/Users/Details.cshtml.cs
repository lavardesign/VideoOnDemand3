using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VOD.Database.Services;
using VOD.Common.Entities;
using VOD.Common.DTOModels;
using VOD.Common.Extensions;

namespace VOD.Admin
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly IDbReadService _dbread;
        private readonly IDbWriteService _dbwrite;
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        public SelectList AvailableCourses { get; set; } = new SelectList(new List<Course>());
        public UserDTO Customer { get; set; }
        [BindProperty, Display(Name = "Available Courses")]
        public int CourseId { get; set; }

        public DetailsModel(IDbReadService dbReadService, IDbWriteService dbWriteService)
        {
            _dbread = dbReadService;
            _dbwrite = dbWriteService;
        }

        private async Task FillViewData(string userId)
        {
            var user = await _dbread.SingleAsync<VODUser>(u => u.Id.Equals(userId));
            Customer = new UserDTO { Id = user.Id, Email = user.Email };
            _dbread.Include<UserCourse>();
            var userCourses = await _dbread.GetAsync<UserCourse>(uc => uc.UserId.Equals(userId));
            var usersCourseIds = userCourses.Select(c => c.CourseId).ToList();
            Courses = userCourses.Select(c => c.Course).ToList();
            var availableCourses = await _dbread.GetAsync<Course>(uc => !usersCourseIds.Contains(uc.Id));
            AvailableCourses = availableCourses.ToSelectList("Id", "Title");
        }

        public async Task OnGetAsync(string id)
        {
            await FillViewData(id);
        }

        public async Task<IActionResult> OnPostAddAsync(string userId)
        {
            try
            {
                _dbwrite.Add(new UserCourse { CourseId = CourseId, UserId = userId });
                var succeeded = await _dbwrite.SaveChangesAsync();
            }
            catch
            { }

            await FillViewData(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int courseId, string userId)
        {
            try
            {
                var userCourse = await _dbread.SingleAsync<UserCourse>(uc => uc.UserId.Equals(userId) &&
                uc.CourseId.Equals(courseId));

                if (userCourse != null)
                {
                    _dbwrite.Delete(userCourse);
                    await _dbwrite.SaveChangesAsync();
                }
            }
            catch
            { }

            await FillViewData(userId);
            return Page();
        }
    }
}