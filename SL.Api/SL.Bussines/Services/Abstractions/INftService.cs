using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface INftService
    {
        Task<Nft> MintNftAsync(string imageId, string userId, string metadataUrl);

        Task<Nft> UpdateNftAsync(string nftId, bool isOnAuction);

        Task<Nft> GetNftByIdAsync(string nftId);

        Task<Nft> TransferNftAsync(string nftId, string newUserId);


    }
}
