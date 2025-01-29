using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class NftMetadata
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } // Название NFT

        [Required]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // Описание NFT

        [Required]
        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        public string Image { get; set; } // Ссылка на изображение NFT
    }
}
