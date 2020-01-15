using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using VOD.Database.Services;
using VOD.Common.DTOModels;

namespace VOD.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;
        [BindProperty]
        public UserDTO Input { get; set; } = new UserDTO();
        [TempData]
        public string Alert { get; set; }

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnGetAsync(string id)
        {
            Alert = string.Empty;
            Input = await _userService.GetUserAsync(id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(Input);

                if (result)
                {
                    Alert = $"User {Input.Email} was updated.";
                    return RedirectToPage("Index");
                }
            }
            return Page();
        }
    }
}
