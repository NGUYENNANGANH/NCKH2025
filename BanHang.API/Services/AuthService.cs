using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BanHang.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email không tồn tại"
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Mật khẩu không đúng"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user, userRoles);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token.Item1,
                Expiration = token.Item2,
                UserId = user.Id,
                UserName = user.UserName,
                Roles = userRoles.ToList(),
                Message = "Đăng nhập thành công"
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email đã tồn tại"
                };
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Đăng ký không thành công: {errors}"
                };
            }

            // Check if the Customer role exists
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Assign the Customer role to the user
            await _userManager.AddToRoleAsync(user, "Customer");

            // Create a cart for the user
            var gioHang = new GioHang
            {
                Id_User = user.Id,
                NgayTao = DateTime.Now
            };
            await _context.GioHangs.AddAsync(gioHang);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Đăng ký thành công"
            };
        }

        private async Task<Tuple<string, DateTime>> GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenExpiration = DateTime.Now.AddHours(3);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: tokenExpiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new Tuple<string, DateTime>(tokenHandler.WriteToken(token), tokenExpiration);
        }
    }

    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> RegisterAsync(RegisterDto model);
    }
} 