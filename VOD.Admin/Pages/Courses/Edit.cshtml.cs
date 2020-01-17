using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using VOD.Database.Services;
using VOD.Common.DTOModels.Admin;
using VOD.Common.Services;
using VOD.Common.Entities;
using VOD.Common.Extensions;

namespace VOD.Admin.Pages.Courses
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAdminService _db;
        [BindProperty]
        public CourseDTO Input { get; set; } = new CourseDTO();
        [TempData]
        public string Alert { get; set; }

        public EditModel(IAdminService db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Alert = string.Empty;
                ViewData["Instructors"] = (await _db.GetAsync<Instructor, InstructorDTO>()).ToSelectList("Id", "Name");
                Input = await _db.SingleAsync<Course, CourseDTO>(s => s.Id.Equals(id));
                return Page();
            }
            catch
            {
                Alert = "You do not have access to this page.";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var succeeded = await _db.UpdateAsync<CourseDTO, Course>(Input);

                if (succeeded)
                {
                    Alert = $"Updated Course: {Input.Title}.";
                    return RedirectToPage("Index");
                }
            }
            ViewData["Instructors"] = (await _db.GetAsync<Instructor, InstructorDTO>()).ToSelectList("Id", "Name");

            return Page();
        }
    }
}
