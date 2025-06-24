using EmailConfirmed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using System.Security.Claims;

namespace EmailConfirmed.Controllers
{
    [Authorize(Policy = "AspAdmin")]
    public class ClaimsController : Controller
    {
        private UserManager<User> _userManager;
        private IAuthorizationService _authService;
        public ClaimsController(UserManager<User> userManager, IAuthorizationService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }
        public async Task<IActionResult> PrivateAccess(string title)
        {
            string[] allowedUsers = { "Игорь" };
            AuthorizationResult authorized = await _authService.AuthorizeAsync(User, allowedUsers, "PrivateAccess");

            if (authorized.Succeeded)
                return View("Index", User?.Claims);
            else
                return new ChallengeResult();
        }
        public ViewResult Index() => View(User?.Claims);
        public ViewResult Create() => View();

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create(string claimType, string claimValue)
        {
            User? user = await _userManager.GetUserAsync(HttpContext.User);
            Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);
            IdentityResult result = await _userManager.AddClaimAsync(user, claim);
            if(result.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(result);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string claimValues)
        {
            User? user = await _userManager.GetUserAsync(HttpContext.User);

            string[] claimValuesArray = claimValues.Split(',');
            string claimType = claimValuesArray[0], claimValue = claimValuesArray[1], claimIssuer = claimValuesArray[2];
            Claim? claim = User.Claims.Where(x => x.Type == claimType && x.Value == claimValue &&  x.Issuer == claimIssuer).FirstOrDefault();

            IdentityResult result = await _userManager.RemoveClaimAsync(user, claim);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(result);
            return View();
        }

        private void Errors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
