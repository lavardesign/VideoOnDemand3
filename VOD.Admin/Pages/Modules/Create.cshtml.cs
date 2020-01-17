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

namespace VOD.Admin.Pages.Modules
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IAdminService _db;
        [BindProperty]
        public ModuleDTO Input { get; set; } = new ModuleDTO();
        [TempData]
        public string Alert { get; set; }

        public CreateModel(IAdminService db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                ViewData["Courses"] = (await _db.GetAsync<Course, CourseDTO>()).ToSelectList("Id", "Title");
                return Page();
            }
            catch
            {
                return RedirectToPage("/Index", new { alert = "You do not have access to this page." });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var succeeded = (await _db.CreateAsync<ModuleDTO, Module>(Input)) > 0;

                if (succeeded)
                {
                    Alert = $"Created a new Module: {Input.Title}.";
                    return RedirectToPage("Index");
                }
            }
            ViewData["Courses"] = (await _db.GetAsync<Course, CourseDTO>()).ToSelectList("Id", "Title");
            return Page();
        }
    }
}
