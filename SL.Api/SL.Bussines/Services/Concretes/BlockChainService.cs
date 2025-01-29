using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Wallet;
using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SL.Bussines.Services.Abstractions;
using Solnet.Rpc.Models;

namespace SL.Bussines.Services.Concretes
{
    public class BlockChainService : IBlockChainService
    {
        public Task<string> CreateMintTransactionAsync(string metadataUrl, string userPublicKey, string nftPublicKey, string mintPublicKey, string tokenAccountPublicKey)
        {
            throw new NotImplementedException();
        }

        public Task LockNftAsync(string tokenId)
        {
            throw new NotImplementedException();
        }

        public Task TransferNftAsync(string tokenId, string newOwnerWalletAddress)
        {
            throw new NotImplementedException();
        }

        public Task UnlockNftAsync(string tokenId)
        {
            throw new NotImplementedException();
        }
    }
}