using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SL.Bussines.Services.Abstractions;
using SL.Core.DTO_s;

namespace SL.Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.WalletAddress) || string.IsNullOrWhiteSpace(dto.Username))
                {
                    return BadRequest("Invalid input data.");
                }

                var user = await _userService.RegisterUserAsync(dto.WalletAddress, dto.Username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{walletAddress}")]
        public async Task<IActionResult> GetUser(string walletAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(walletAddress))
                {
                    return BadRequest("Wallet address cannot be null or empty.");
                }

                var user = await _userService.GetUserByWalletAsync(walletAddress);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(404, new { Error = ex.Message });
            }
        }

        [HttpPut("change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.WalletAddress) || string.IsNullOrWhiteSpace(dto.NewUsername))
                {
                    return BadRequest("Invalid input data.");
                }

                var updatedUser = await _userService.ChangeUsernameAsync(dto.WalletAddress, dto.NewUsername);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPut("change-wallet")]
        public async Task<IActionResult> ChangeWalletAddress([FromBody] ChangeWalletAddressDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.NewWalletAddress))
                {
                    return BadRequest("Invalid input data.");
                }

                var updatedUser = await _userService.ChangeWalletAddressAsync(dto.UserId, dto.NewWalletAddress);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
