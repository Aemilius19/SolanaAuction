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
    public class Nft : BaseEntity
    {
        [Required]
        public string TokenId { get; set; } // Blockchain token ID, уникальный идентификатор токена на блокчейне

        [Required]
        public string MetadataUrl { get; set; } // Ссылка на метаданные NFT (например, IPFS JSON)

        [Required]
        public string UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public User Owner { get; set; } // Связь с владельцем NFT

        public decimal? SalePrice { get; set; } // Цена продажи NFT (если выставлено на продажу)

        public bool IsOnAuction { get; set; } = false; // Флаг, указывающий, участвует ли NFT в аукционе

        public string BlockchainTransactionHash { get; set; } // Хеш транзакции блокчейна для отслеживания операций

        public DateTime? MintedAt { get; set; } // Время минтинга NFT

        public bool IsTransferred { get; set; } = false; // Флаг, указывающий, было ли NFT передано другому пользователю
    }
}

