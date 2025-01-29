using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.DTO_s
{
    public class BidDto
    {
        public string AuctionId { get; set; }
        public decimal BidPrice { get; set; }
        public string BidderWalletAddress { get; set; }
    }
}
