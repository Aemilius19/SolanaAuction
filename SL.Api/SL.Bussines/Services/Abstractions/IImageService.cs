using SL.Application.Repositories.Abstractions;
using SL.Core.DTO_s;
using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface IImageService
    {
        Task<string> GenerateImageAsync(string prompt);

        Task<Image> SaveImageAsync(SaveImageDto dto);
        Task<bool> DeleteImageAsync(string imageId, string userId);

        Task<ICollection<Image>> GetUserImages(string userId);

        Task<Image> UploadIMage(SaveImageDto dto);
    }
}
