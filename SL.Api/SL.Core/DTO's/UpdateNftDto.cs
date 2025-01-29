using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class UpdateNftDto
    {
        public string NftId { get; set; } // ID NFT
        public bool IsOnAuction { get; set; } // Статус аукциона
    }
}
