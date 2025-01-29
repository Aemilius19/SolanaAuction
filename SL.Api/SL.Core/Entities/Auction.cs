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
    public class Auction : BaseEntity
    {
        [Required]
        public string NftId { get; set; } // Foreign key to NFT

        [ForeignKey("NftId")]
        public Nft Nft { get; set; }

        [Required]
        public decimal StartPrice { get; set; } // Starting price

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } // Auction end date

        public string? HighestBidderWalletAddress { get; set; } // Highest bidder's wallet
        public decimal? HighestBid { get; set; } // Highest bid amount

        public bool IsActive { get; set; } = true;
    }
}
