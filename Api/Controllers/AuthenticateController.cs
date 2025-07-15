// using Domain.Constants;
// using Domain.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using WebApplication1.Dto;
//
// [ApiController]
// [Route("auth")]
// public class AuthenticateController : ControllerBase
// {
//     // Create private fields to hold the injected services
//
//     private readonly IConfiguration _configuration;
//
//
//
//     // Now you can use _userManager and _roleManager in your methods
//     [HttpPost]
//     [Route("login")]
//     public async Task<IActionResult> Login(LoginDto loginDtoDto)
//     {
//         var user = await _userManager.FindByEmailAsync(loginDtoDto.Email);
//         if (user != null && await _userManager.CheckPasswordAsync(user, loginDtoDto.Password))
//         {
//             // Use the injected UserManager again to check the role
//             if (!await _userManager.IsInRoleAsync(user, AppClaims.AdminRole))
//             {
//                 return Forbid("You do not have administrative access.");
//             }
//
//             return Ok(/* ... token ... */);
//         }
//         return Unauthorized();
//     }
//     
//     [HttpPost]
//     [Route("register")]
//     public async Task<IActionResult> Register(Register registerDto)
//     {
//         var user = await _userManager.FindByEmailAsync(registerDto.Email);
//
//         if (user != null )
//         {
//             return Conflict("User already exists.");
//         }
//         
//         var createRes = await _userManager.CreateAsync(
//             new User
//             {
//                 Email = registerDto.Email, 
//                 FirstName = registerDto.FirstName, 
//                 LastName = registerDto.LastName
//             }
//             ,registerDto.Password);
//
//         if (createRes.Succeeded)
//         {
//             user = await _userManager.FindByEmailAsync(registerDto.Email);
//             ArgumentNullException.ThrowIfNull(user);
//             await _userManager.AddToRoleAsync(user, AppClaims.UserRole);
//             return Ok();
//             
//         } else if (createRes.Errors.Any())
//         {
//             string resText = "";
//             createRes.Errors.ToList().ForEach(x => resText += x.Description + "\n");
//             return Unauthorized(resText);
//         }
//         
//         return Unauthorized();
//     }
// }