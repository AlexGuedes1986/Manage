using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace BioTech.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin, Project Manager")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IBillingRoleService _billingRoleService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, IUserService userService, IBillingRoleService billingRoleService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _billingRoleService = billingRoleService;
            // instantiate a new model
            Input = new InputModel();

            Input.WebsiteRoles = _userService.GetWebsiteRoleUsers();
            Input.BillingRoles = _billingRoleService.GetBillingRolesSelectListItems();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [RegularExpression("^[a-zA-Z0-9.]+$", ErrorMessage = "Username should be composed only of letters and numbers")]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            public List<RoleUser> WebsiteRoles { get; set; }

            [Display(Name = "Is Active")]
            public bool IsActive { get; set; }
            public IEnumerable<SelectListItem> BillingRoles { get; set; }
            [Required(ErrorMessage = "Please select a Billing Role")]
            public int BillingRoleId { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var websiteRoleSelected = false;
            //BSS just validating Website Roles
            foreach (var role in Input.WebsiteRoles)
            {
                if (role.IsSelected == true)
                {
                    websiteRoleSelected = true;
                    break;
                }
            }
            //BSS just validating Website Roles
            if (websiteRoleSelected == false)
            {
                ModelState.AddModelError("Input.Roles", "Please select a Website Role");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    IsActive = Input.IsActive,
                    PhoneNumber = Input.PhoneNumber,
                    BillingRoleId = Input.BillingRoleId
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    foreach (var role in Input.WebsiteRoles)
                    {
                        if (role.IsSelected == true)
                        {
                            // add them to the the selected role
                            await _userManager.AddToRoleAsync(user, role.Name);
                        }
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return LocalRedirect("/Admin");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            Input.WebsiteRoles = _userService.GetWebsiteRoleUsers();
            Input.BillingRoles = _billingRoleService.GetBillingRolesSelectListItems();
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
