using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class SaveImageDto
    {
        public string ImageUrl { get; set; } // URL of the image to be saved
        public string UserId { get; set; }   // ID of the user saving the image

        public string Name { get; set; } // Название изображения

        public string Description { get; set; } // Описание изображения
    }
}
