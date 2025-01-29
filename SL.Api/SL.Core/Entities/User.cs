using Microsoft.AspNetCore.Identity;
using SL.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Core.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string WalletAddress { get; set; } // Phantom Wallet address

        public string Username { get; set; } // Optional username for display

        public ICollection<Nft> Nfts { get; set; } = new List<Nft>();
        public ICollection<Image> Images { get; set; } = new List<Image>();

        public string Role { get; set; } = "user";
    }
}
