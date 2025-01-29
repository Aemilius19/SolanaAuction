using SL.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.Entities
{
    public class Image : BaseEntity
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public User Owner { get; set; }

        public bool IsMinted { get; set; } = false;

        public string Name { get; set; } // Название изображения

        public string Description { get; set; } // Описание изображения


    }
}
