using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Tên vai trò không được để trống");
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest($"Vai trò '{roleName}' đã tồn tại");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                return Ok($"Vai trò '{roleName}' đã được tạo thành công");
            }

            return BadRequest($"Không thể tạo vai trò '{roleName}'");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {userId}");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpPost("user/{userId}")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Tên vai trò không được để trống");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {userId}");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest($"Vai trò '{roleName}' không tồn tại");
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return BadRequest($"Người dùng đã có vai trò '{roleName}'");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"Người dùng đã được thêm vào vai trò '{roleName}'");
            }

            return BadRequest($"Không thể thêm người dùng vào vai trò '{roleName}'");
        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Tên vai trò không được để trống");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {userId}");
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest($"Vai trò '{roleName}' không tồn tại");
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                return BadRequest($"Người dùng không có vai trò '{roleName}'");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"Đã xóa vai trò '{roleName}' khỏi người dùng");
            }

            return BadRequest($"Không thể xóa vai trò '{roleName}' khỏi người dùng");
        }
    }
} 