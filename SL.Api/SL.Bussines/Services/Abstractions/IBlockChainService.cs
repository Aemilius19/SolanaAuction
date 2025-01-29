using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface IBlockChainService
    {
        Task<string> CreateMintTransactionAsync(
        string metadataUrl,
        string userPublicKey,
        string nftPublicKey,
        string mintPublicKey,
        string tokenAccountPublicKey
        );

        Task LockNftAsync(string tokenId);

        Task UnlockNftAsync(string tokenId);

        Task TransferNftAsync(string tokenId, string newOwnerWalletAddress);
    }
}
