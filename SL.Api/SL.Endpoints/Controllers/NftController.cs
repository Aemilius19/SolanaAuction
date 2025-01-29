using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SL.Bussines.Services.Abstractions;
using SL.Bussines.Services.Concretes;
using SL.Core.DTO_s;

namespace SL.Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NftController : ControllerBase
    {
        private readonly INftService _nftService;
        private readonly IIpfsService _ipfsService;
        private readonly IBlockChainService _blockChainService;


        public NftController(INftService nftService, IIpfsService ipfsService)
        {
            _nftService = nftService;
            _ipfsService = ipfsService;
        }


        [HttpPost("upload-metadata")]
        public async Task<IActionResult> UploadMetadata([FromBody] NftMetadata metadata)
        {
            try
            {
                // Автоматическая валидация благодаря [ApiController]
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Загрузка метаданных через сервис
                var ipfsUri = await _ipfsService.UploadMetadataAsync(metadata);

                return Ok(new { IpfsUrl = ipfsUri });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
            }
        }
        [HttpPost("mint")]
        public async Task<IActionResult> MintNft([FromBody] MintNftDto dto)
        {
            try
            {
                // dto содержит: userPublicKey, nftPublicKey, mintPublicKey, tokenAccountPublicKey, metadataUrl
                // Вызываем сервис, чтобы создать транзакцию
                string base64Tx = await _blockChainService.CreateMintTransactionAsync(
                    dto.MetadataUrl,
                    dto.UserPublicKey,
                    dto.NftPublicKey,
                    dto.MintPublicKey,
                    dto.TokenAccountPublicKey
                );

                // Возвращаем raw-транзакцию
                return Ok(new
                {
                    transaction = base64Tx
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Обновление состояния NFT
        [HttpPut("update")]
        public async Task<IActionResult> UpdateNft([FromBody] UpdateNftDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.NftId))
                {
                    return BadRequest("Invalid input data.");
                }

                var nft = await _nftService.UpdateNftAsync(dto.NftId, dto.IsOnAuction);
                return Ok(nft);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Передача NFT другому пользователю
        [HttpPut("transfer")]
        public async Task<IActionResult> TransferNft([FromBody] TransferNftDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.NftId) || string.IsNullOrWhiteSpace(dto.NewUserId))
                {
                    return BadRequest("Invalid input data.");
                }

                var nft = await _nftService.TransferNftAsync(dto.NftId, dto.NewUserId);
                return Ok(nft);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
