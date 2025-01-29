using Microsoft.IdentityModel.Tokens;
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
    public class NftService : INftService
    {

        private readonly INftRepository _nftRepository;
        private readonly IBlockChainService _blockchainService;

        public NftService(INftRepository nftRepository, IBlockChainService blockchainService)
        {
            _nftRepository = nftRepository;
            _blockchainService = blockchainService;
        }

        public async Task<Nft> MintNftAsync(string imageId, string userId, string metadataUrl)
        {
            

            // Сохранение данных в базе
            var nft = new Nft
            {
                //TokenId = tokenId,
                MetadataUrl = metadataUrl,
                UserId = userId,
               // BlockchainTransactionHash = transactionHash,
                MintedAt = DateTime.UtcNow
            };

            return await _nftRepository.Add(nft);
        }

        public async Task<Nft> UpdateNftAsync(string nftId, bool isOnAuction)
        {
            var nft = await _nftRepository.GetById(nftId);
            if (nft == null)
            {
                throw new Exception("NFT not found.");
            }

            if (isOnAuction)
            {
                await _blockchainService.LockNftAsync(nft.TokenId);
            }
            else
            {
                await _blockchainService.UnlockNftAsync(nft.TokenId);
            }

            nft.IsOnAuction = isOnAuction;
            return await _nftRepository.Update(nft);
        }

        public async Task<Nft> TransferNftAsync(string nftId, string newUserId)
        {
            var nft = await _nftRepository.GetById(nftId);
            if (nft == null)
            {
                throw new Exception("NFT not found.");
            }

            if (nft.IsOnAuction)
            {
                throw new Exception("Cannot transfer NFT while it is on auction.");
            }

            var newOwnerWalletAddress = "mock-wallet-address"; // Получите из базы или другого источника
            await _blockchainService.TransferNftAsync(nft.TokenId, newOwnerWalletAddress);

            nft.UserId = newUserId;
            nft.IsTransferred = true;
            return await _nftRepository.Update(nft);
        }

        public async Task<Nft> GetNftByIdAsync(string nftId)
        {
            if (nftId.IsNullOrEmpty())
            {
                throw new Exception("nft id can not be null");
            }

            var nft=await _nftRepository.GetById(nftId);
            if(nft == null)
            {
                throw new Exception(" Dont find nft with that id");
            }
            return nft;
        }
    }
}
