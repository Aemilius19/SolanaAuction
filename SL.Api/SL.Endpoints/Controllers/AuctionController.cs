using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SL.Bussines.Services.Abstractions;
using SL.Core.DTO_s;

namespace SL.Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.NftId) || dto.StartPrice <= 0)
                {
                    return BadRequest("Invalid input data.");
                }

                var auction = await _auctionService.CreateAuctionAsync(dto.NftId, dto.StartPrice, dto.UserId);
                return Ok(auction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("bid")]
        public async Task<IActionResult> PlaceBid([FromBody] BidDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.AuctionId) || dto.BidPrice <= 0)
                {
                    return BadRequest("Invalid input data.");
                }

                var auction = await _auctionService.UpdateAuctionAsync(dto.AuctionId, dto.BidPrice, dto.BidderWalletAddress);
                return Ok(auction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("close")]
        public async Task<IActionResult> CloseAuction([FromBody] CloseAuctionDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.AuctionId) || string.IsNullOrWhiteSpace(dto.WinnerWalletAddress))
                {
                    return BadRequest("Invalid input data.");
                }

                await _auctionService.CloseAuctionAsync(dto.AuctionId, dto.WinnerWalletAddress);
                return Ok(new { Message = "Auction closed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
