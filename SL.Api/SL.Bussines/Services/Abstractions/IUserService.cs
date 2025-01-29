using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Bussines.Services.Abstractions
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(string walletAdress,string userName);

        Task<User> GetUserByWalletAsync(string walletAdress);
        Task<User> ChangeUsernameAsync(string userId, string newUsername);

        Task<User> ChangeWalletAddressAsync(string userId, string newWalletAddress);
    }
}
