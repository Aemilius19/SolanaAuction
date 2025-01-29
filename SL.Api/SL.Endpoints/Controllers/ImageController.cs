using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SL.Bussines.Services.Abstractions;
using SL.Core.DTO_s;
using SL.Core.Entities;

namespace SL.Endpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("{userId}")]

        public async Task<IActionResult> GetUserImages(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var images = await _imageService.GetUserImages(userId);
                return Ok(images); // 200 OK с коллекцией (даже пустой)
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Error = ex.Message }); // 400
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] ImageGenerationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Prompt))
            {
                return BadRequest("Prompt cannot be null or empty.");
            }

            if (dto.Prompt.Length > 100)
            {
                return BadRequest("Prompt length cannot exceed 100 characters.");
            }

            var imageUrl = await _imageService.GenerateImageAsync(dto.Prompt);
            return Ok(new ImageGenerationResponseDto { ImageUrl = imageUrl });
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveImage([FromBody] SaveImageDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.ImageUrl) || string.IsNullOrWhiteSpace(dto.UserId))
                {
                    return BadRequest("Image URL and User ID cannot be null or empty.");
                }

                var savedImage = await _imageService.SaveImageAsync(dto);
                return Ok(savedImage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(
      [FromForm] IFormFile image,
      [FromForm] string userId,
      [FromForm] string name,
      [FromForm] string description)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return BadRequest("Файл изображения не выбран");

                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest("User ID обязателен");

                var imageUrl = await SaveFile(image, userId);
                var dto = new SaveImageDto
                {
                    UserId = userId,
                    Name = name ?? "Без названия",
                    Description = description ?? "Без описания",
                    ImageUrl = imageUrl
                };

                var savedImage = await _imageService.UploadIMage(dto);
                return Ok(new
                {
                    Url = savedImage.Url,
                    Name = savedImage.Name
                });
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка загрузки: {ex}");
                return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
            }
        }

        private async Task<string> SaveFile(IFormFile file, string userId)
        {
            var userFolder = Path.Combine("wwwroot/images", userId);
            Directory.CreateDirectory(userFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(userFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{userId}/{fileName}";
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(
     string imageId,
     [FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageId) || string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest(new { Error = "Image ID and User ID required" });
                }

                var isDeleted = await _imageService.DeleteImageAsync(imageId, userId);

                return isDeleted
                    ? Ok(new { Message = "Image deleted successfully" })
                    : NotFound(new { Error = "Image not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
