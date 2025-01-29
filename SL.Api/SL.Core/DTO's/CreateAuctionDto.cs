using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class CreateAuctionDto
    {
        public string NftId { get; set; }
        public decimal StartPrice { get; set; }
        public string UserId { get; set; }
    }
}
