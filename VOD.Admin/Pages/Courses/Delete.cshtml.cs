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

namespace VOD.Admin.Pages.Courses
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IAdminService _db;
        [BindProperty]
        public CourseDTO Input { get; set; } = new CourseDTO();
        [TempData]
        public string Alert { get; set; }

        public DeleteModel(IAdminService db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Alert = string.Empty;
                Input = await _db.SingleAsync<Course, CourseDTO>(s => s.Id.Equals(id), true);
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
            var id = Input.Id;

            if (ModelState.IsValid)
            {
                var succeeded = await _db.DeleteAsync<Course>(d => d.Id.Equals(id));

                if (succeeded)
                {
                    Alert = $"Deleted Course: {Input.Title}.";
                    return RedirectToPage("Index");
                }
            }

            Input = await _db.SingleAsync<Course, CourseDTO>(s => s.Id.Equals(id), true);

            return Page();
        }
    }
}
