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

namespace VOD.Admin.Pages.Instructors
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAdminService _db;
        [BindProperty]
        public InstructorDTO Input { get; set; } = new InstructorDTO();
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
                Input = await _db.SingleAsync<Instructor, InstructorDTO>(s => s.Id.Equals(id));
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
                var succeeded = await _db.UpdateAsync<InstructorDTO, Instructor>(Input);

                if (succeeded)
                {
                    Alert = $"Updated instructor: {Input.Name}.";
                    return RedirectToPage("Index");
                }
            }
            return Page();
        }
    }
}
