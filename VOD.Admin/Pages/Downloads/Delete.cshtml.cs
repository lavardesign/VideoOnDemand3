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

namespace VOD.Admin.Pages.Downloads
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IAdminService _db;
        [BindProperty]
        public DownloadDTO Input { get; set; } = new DownloadDTO();
        [TempData]
        public string Alert { get; set; }

        public DeleteModel(IAdminService db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id, int courseId, int moduleId)
        {
            try
            {
                Input = await _db.SingleAsync<Download, DownloadDTO>(s => s.Id.Equals(id) && s.ModuleId.Equals(moduleId) && s.CourseId.Equals(courseId), true);
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
            int id = Input.Id, moduleId = Input.ModuleId, courseId = Input.CourseId;

            if (ModelState.IsValid)
            {
                var succeeded = await _db.DeleteAsync<Download>(d => d.Id.Equals(id) && d.ModuleId.Equals(moduleId) && d.CourseId.Equals(courseId));

                if (succeeded)
                {
                    Alert = $"Deleted Download: {Input.Title}.";
                    return RedirectToPage("Index");
                }
            }

            Input = await _db.SingleAsync<Download, DownloadDTO>(s => s.Id.Equals(id) && s.ModuleId.Equals(moduleId) && s.CourseId.Equals(courseId), true);

            return Page();
        }
    }
}
