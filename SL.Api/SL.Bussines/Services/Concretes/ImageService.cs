using Microsoft.EntityFrameworkCore;
using SL.Application.Repositories.Abstractions;
using SL.Bussines.Services.Abstractions;
using SL.Core.DTO_s;
using SL.Core.Entities;
using SL.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Concretes
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly OpenAiSettings _openAiSettings;

        public ImageService(IImageRepository imageRepository, OpenAiSettings openAiSettings)
        {
            _imageRepository = imageRepository;
            _openAiSettings = openAiSettings;
        }

        public async Task<bool> DeleteImageAsync(string imageId, string userId)
        {
            if (string.IsNullOrWhiteSpace(imageId) || string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("Image ID and User ID cannot be empty");
            }

            var image = await _imageRepository.GetById(imageId);
            if (image == null) return false;

            // Проверка прав доступа
            if (image.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            // Формирование физического пути к файлу
            var filePath = Path.Combine("wwwroot", image.Url.TrimStart('/'));

            // Удаление файла
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Удаление из базы данных
            return await _imageRepository.DeleteById(imageId);
        }

        public async Task<string> GenerateImageAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be null or empty.");
            }
            var styledPrompt = $"Create a  NFT-style image of {prompt}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiSettings.ApiKey);

            var requestBody = new
            {
                prompt = styledPrompt,
                n = 1, 
                size = "1024x1024" 
            };

            var response = await httpClient.PostAsJsonAsync(_openAiSettings.BaseUrl, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI API error: {error}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<OpenAiResponse>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Handles mismatched casing in property names
            });

            if (responseObject?.Data == null || !responseObject.Data.Any())
            {
                throw new Exception("No image URL returned.");
            }

            return responseObject.Data.First().Url;
        }

        public async Task<ICollection<Image>> GetUserImages(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null");

            return await _imageRepository.GetAll()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Image> SaveImageAsync(SaveImageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Image Name and DEscription cannot be null or empty.");
            }

            var userFolderPath = Path.Combine("wwwroot/images", dto.UserId);
            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(userFolderPath, fileName);

            // Сохраняем изображение
            using (var client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(dto.ImageUrl);
                await File.WriteAllBytesAsync(filePath, imageBytes);
            }
            var relativePath = $"/images/{dto.UserId}/{fileName}";


            var image = new Image
            {
                Url = relativePath,
                UserId = dto.UserId,
                IsMinted = false,
                Name=dto.Name,
                Description=dto.Description,
             
            };

            // Save the image information to the database
            return await _imageRepository.Add(image);
        }

        public async Task<Image> UploadIMage(SaveImageDto dto)
        {
            var image = new Image();
            image.Url = dto.ImageUrl;
            image.Description = dto.Description;
            image.UserId = dto.UserId;
            image.Name = dto.Name;

            return await _imageRepository.Add(image);
           
        }
    }
}
