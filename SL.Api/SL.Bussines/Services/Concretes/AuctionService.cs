using SL.Application.Repositories.Abstractions;
using SL.Bussines.Services.Abstractions;
using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Concretes
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly INftService _nftService;
        private readonly IBlockChainService _blockchainService;

        public AuctionService(
            IAuctionRepository auctionRepository,
            INftService nftService,
            IBlockChainService blockchainService)
        {
            _auctionRepository = auctionRepository;
            _nftService = nftService;
            _blockchainService = blockchainService;
        }
        public async Task CloseAuctionAsync(string auctionId, string winnerWalletAddress)
        {
            var auction = await _auctionRepository.GetById(auctionId);
            if (auction == null || !auction.IsActive)
                throw new Exception("Auction not found or already closed.");

            // Закрыть аукцион
            auction.IsActive = false;
            await _auctionRepository.Update(auction);

            // Разблокировать NFT
            await _nftService.UpdateNftAsync(auction.NftId, false);

            // Передать NFT победителю
            //исправить ошибку 
            //await _blockchainService.TransferNft(auction.NftId, winnerWalletAddress);
        }

        public async Task<Auction> CreateAuctionAsync(string nftId, decimal startPrice, string userId)
        {
            var nft = await _nftService.GetNftByIdAsync(nftId);
            if (nft == null || nft.IsOnAuction)
                throw new Exception("NFT is not valid or already on auction.");

            // Заблокировать NFT (выставить на аукцион)
            await _nftService.UpdateNftAsync(nftId, true);

            var auction = new Auction
            {
                NftId = nftId,
                StartPrice = startPrice,
                StartDate = DateTime.UtcNow,
                IsActive = true
            };

            return await _auctionRepository.Add(auction);
        }

        public async Task<Auction> GetAuctionAsync(string auctionId)
        {
            var auction = await _auctionRepository.GetById(auctionId);
            if (auction == null)
                throw new Exception("Auction not found.");

            return auction;
        }

        public async Task<Auction> UpdateAuctionAsync(string auctionId, decimal bidPrice, string bidderWalletAddress)
        {
            var auction = await _auctionRepository.GetById(auctionId);
            if (auction == null || !auction.IsActive)
                throw new Exception("Auction not found or inactive.");

            if (bidPrice <= auction.HighestBid)
                throw new Exception("Bid price must be higher than the current highest bid.");

            auction.HighestBid = bidPrice;
            auction.HighestBidderWalletAddress = bidderWalletAddress;

            return await _auctionRepository.Update(auction);
        }
    }
}
