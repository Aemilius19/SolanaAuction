using Microsoft.EntityFrameworkCore;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> ChangeUsernameAsync(string wallletAdress, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(wallletAdress) || string.IsNullOrWhiteSpace(newUsername))
            {
                throw new ArgumentException("User ID and username cannot be null or empty.");
            }

            // Проверяем, существует ли пользователь с новым username
            var existingUser = await _userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Username == newUsername);

            if (existingUser != null)
            {
                throw new Exception("Username already in use.");
            }

            // Находим пользователя по ID
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(u => u.WalletAddress == wallletAdress);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Обновляем username
            user.Username = newUsername;
            return await _userRepository.Update(user);
        }

        public async Task<User> ChangeWalletAddressAsync(string userId, string newWalletAddress)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newWalletAddress))
            {
                throw new ArgumentException("User ID and wallet address cannot be null or empty.");
            }

            // Проверяем, существует ли пользователь с новым wallet address
            var existingUser = await _userRepository.GetAll()
                .FirstOrDefaultAsync(u => u.WalletAddress == newWalletAddress);

            if (existingUser != null)
            {
                throw new Exception("Wallet address already in use.");
            }

            // Находим пользователя по ID
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Обновляем wallet address
            user.WalletAddress = newWalletAddress;
            return await _userRepository.Update(user);
        }

        public async Task<User> GetUserByWalletAsync(string walletAddress)
        {
            // Проверка на пустое или null значение walletAddress
            if (string.IsNullOrWhiteSpace(walletAddress))
            {
                throw new ArgumentException("Wallet address cannot be null or empty.", nameof(walletAddress));
            }

            var existingUser = await _userRepository
                    .GetAll("Images","Nfts")
                    .Where(u => u.WalletAddress == walletAddress)
                    .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            return existingUser;
        }

        public async Task<User> RegisterUserAsync(string walletAddress, string userName)
        {
            // Проверка на пустое или null значение walletAddress
            if (string.IsNullOrWhiteSpace(walletAddress))
            {
                throw new ArgumentException("Wallet address cannot be null or empty.", nameof(walletAddress));
            }

            // Проверка на пустое или null значение userName
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(userName));
            }

            var existingUser = await _userRepository.GetAll()
                .Where(u => u.WalletAddress == walletAddress)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                throw new Exception("User already registered.");
            }

            // Создать нового пользователя
            var user = new User
            {
                WalletAddress = walletAddress,
                Username = userName
            };

            return await _userRepository.Add(user);
        }
    }
}
