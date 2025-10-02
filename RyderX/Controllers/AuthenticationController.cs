using Google.Apis.Auth; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RyderX_Server.Authentication;
using RyderX_Server.DTO.UpdateDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // Helper method to generate JWT
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["jwt:Secret"])
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:ValidIssuer"],
                audience: _configuration["jwt:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Register new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    return BadRequest(new Response { Status = "Error", Message = "User already exists!" });
                }

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                }

                if (!await _roleManager.RoleExistsAsync(model.Role))
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));

                await _userManager.AddToRoleAsync(user, model.Role);

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = $"An error occurred: {ex.InnerException?.Message ?? ex.Message}" });
            }
        }

        // Login and generate JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = await GenerateJwtToken(user);
                    var roles = await _userManager.GetRolesAsync(user);

                    return Ok(new
                    {
                        username = user.UserName,
                        token,
                        expiration = DateTime.UtcNow.AddHours(5),
                        roles
                    });
                }

                return Unauthorized(new Response { Status = "Error", Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = $"An error occurred: {ex.Message}" });
            }
        }

        // Login with Google OAuth
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthModel model)
        {
            try
            {
                if (model.Provider.ToLower() != "google")
                    return BadRequest(new { Message = "Only Google OAuth supported right now." });

                // Validate Google ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    model.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                    });

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Provider = model.Provider,
                        ProviderUserId = payload.Subject,
                        AvatarUrl = payload.Picture,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return StatusCode(500, new { Message = "Failed to create external user" });

                    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }

                var token = await GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    username = user.UserName,
                    token,
                    expiration = DateTime.UtcNow.AddHours(5),
                    roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "OAuth login failed", Details = ex.Message });
            }
        }

        // Update current user profile
        [HttpPut("profile")]
        [Authorize(Roles = "User,Admin,Agent")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new { Message = "User not found" });

                if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;

                if (!string.IsNullOrEmpty(dto.DriverLicenseNumber)) user.DriverLicenseNumber = dto.DriverLicenseNumber;
                if (dto.LicenseExpiryDate.HasValue) user.LicenseExpiryDate = dto.LicenseExpiryDate.Value;
                if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;

                if (!string.IsNullOrEmpty(dto.Street)) user.Street = dto.Street;
                if (!string.IsNullOrEmpty(dto.City)) user.City = dto.City;
                if (!string.IsNullOrEmpty(dto.State)) user.State = dto.State;
                if (!string.IsNullOrEmpty(dto.ZipCode)) user.ZipCode = dto.ZipCode;
                if (!string.IsNullOrEmpty(dto.Country)) user.Country = dto.Country;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return StatusCode(500, new { Message = "Failed to update profile" });

                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating profile", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize(Roles = "User,Admin,Agent")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new { Message = "User not found" });

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.DriverLicenseNumber,
                    user.LicenseExpiryDate,
                    user.DateOfBirth,
                    user.Street,
                    user.City,
                    user.State,
                    user.ZipCode,
                    user.Country,
                    user.Provider,
                    user.AvatarUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching profile", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // Admin: Get user by ID
        [HttpGet("user/{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { Message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.DriverLicenseNumber,
                user.LicenseExpiryDate,
                user.DateOfBirth,
                user.Street,
                user.City,
                user.State,
                user.ZipCode,
                user.Country,
                user.Provider,
                user.AvatarUrl
            });
        }

        // Admin: Get user by Email
        [HttpGet("user/by-email/{email}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new { Message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.DriverLicenseNumber,
                user.LicenseExpiryDate,
                user.DateOfBirth,
                user.Street,
                user.City,
                user.State,
                user.ZipCode,
                user.Country,
                user.Provider,
                user.AvatarUrl
            });
        }
    }
}
