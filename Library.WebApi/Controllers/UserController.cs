using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Library.Infrastructure.Entities;
using Library.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public UserController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task LogOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(loginModel);
            }

            var result = await _signInManager.PasswordSignInAsync(loginModel.UserName,
                loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == loginModel.UserName);
                var token = await GenerateJwtTokenAsync(appUser);
                return Ok(token);
            }
            return BadRequest("Invalid login attempt.");

        }

        // create token
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>{
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            // get options
            var jwtAppSettingOptions = _configuration.GetSection("JwtIssuerOptions");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(jwtAppSettingOptions["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                jwtAppSettingOptions["JwtIssuer"],
                jwtAppSettingOptions["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// You can pass role name in this scenario, just assign roleid
        /// </summary>
        /// <param name="saveModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveModel saveModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(saveModel.UserId);
                IdentityResult identityResult;
                if (user != null)
                {
                    identityResult = await _userManager.UpdateAsync(user);
                }
                else
                {
                    user = new ApplicationUser { UserName = saveModel.Email, Email = saveModel.Email };
                    identityResult = await _userManager.CreateAsync(user, saveModel.Password);
                }

                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (saveModel.RoleModelIds.Any())
                    {
                        IList<string> existRole = await _userManager.GetRolesAsync(user);

                        foreach (var roleModel in saveModel.RoleModelIds)
                        {
                            IdentityRole selectedIdentityRole = await _roleManager.FindByIdAsync(roleModel);

                            if (selectedIdentityRole != null)
                            {
                                if (!existRole.Any(x => x == selectedIdentityRole.Name))
                                {
                                    IdentityResult newRoleResult = await _userManager.AddToRoleAsync(user, selectedIdentityRole.Name);
                                    if (newRoleResult.Succeeded)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest(identityResult.Errors.Any() ? identityResult.Errors.FirstOrDefault().Description : string.Empty);
                }
            }
            else
            {
                return BadRequest(saveModel);
            }
        }

        [HttpGet(Name = "GetRoles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return new ObjectResult(roles);
        }

        [HttpGet("{id}", Name = "GetRole")]
        public async Task<IActionResult> GetRole(string id)
        {
            IdentityRole selectedIdentityRole = await _roleManager.FindByIdAsync(id);
            if (selectedIdentityRole == null)
            {
                return NotFound();
            }
            else
            {
                return new ObjectResult(selectedIdentityRole);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRole([FromBody] RoleModel roleModel)
        {
            var role = await _roleManager.FindByIdAsync(roleModel.RoleId);

            if (role == null)
            {
                bool x = await _roleManager.RoleExistsAsync(roleModel.Name);
                if (!x)
                {
                    await _roleManager.CreateAsync(new IdentityRole() { Name = roleModel.Name });
                }
            }
            else
            {
                role.Name = roleModel.Name;
                await _roleManager.UpdateAsync(role);
            }
            return Ok();
        }
    }
}