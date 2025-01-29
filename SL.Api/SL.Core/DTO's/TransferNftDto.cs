using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class TransferNftDto
    {
        public string NftId { get; set; } // ID NFT
        public string NewUserId { get; set; } // ID нового владельца
    }
}
