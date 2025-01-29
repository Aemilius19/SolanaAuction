using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface IAuctionService
    {
        Task<Auction> CreateAuctionAsync(string nftId, decimal startPrice, string userId);

        Task<Auction> UpdateAuctionAsync(string auctionId, decimal bidPrice, string bidderWalletAddress);

        Task CloseAuctionAsync(string auctionId, string winnerWalletAddress);

        Task<Auction> GetAuctionAsync(string auctionId);
    }
}
