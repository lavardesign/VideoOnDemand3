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
    public class IndexModel : PageModel
    {
        private readonly IAdminService _db;
        public IEnumerable<CourseDTO> Items = new List<CourseDTO>();
        [TempData]
        public string Alert { get; set; }

        public IndexModel(IAdminService db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Items = await _db.GetAsync<Course, CourseDTO>(true);
                return Page();
            }
            catch
            {
                Alert = "You do not have access to this page.";
                return RedirectToPage("/Index");
            }
        }
    }
}
