document.getElementById("connectWallet").addEventListener("click", async () => {
    try {
        // Проверяем наличие Phantom Wallet
        const provider = window.phantom?.solana;
        if (!provider?.isPhantom) {
            window.open("https://phantom.app/", "_blank");
            return;
        }

        // Подключаем кошелек и получаем адрес
        const { publicKey } = await provider.connect();
        const shortAddress = `${publicKey.toString().slice(0, 6)}...${publicKey.toString().slice(-4)}`;
        
        // Обновляем кнопку
        document.getElementById("connectWallet").textContent = `✔️ ${shortAddress}`;

    } catch (error) {
        alert("Ошибка подключения: " + error.message);
    }
});

document.addEventListener("DOMContentLoaded", async () => {
    try {
        // 1. Проверка Phantom Wallet
        const provider = window.phantom?.solana;
        if (!provider?.isPhantom) {
            showError('Установите Phantom Wallet');
            window.open('https://phantom.app/', '_blank');
            return;
        }

        // 2. Подключение кошелька
        const { publicKey } = await provider.connect();
        const walletAddress = publicKey.toString();
        const shortAddress = `${publicKey.toString().slice(0, 6)}...${publicKey.toString().slice(-4)}`;
        console.log('[DEBUG] Кошелек подключен:', walletAddress);
        // Обновляем кнопку
        document.getElementById("connectWallet").textContent = `✔️ ${shortAddress}`;

        // 3. Загрузка данных пользователя
        try {
            const userData = await fetchUserData(walletAddress);
            showUserProfile(userData, walletAddress);
        } catch (error) {
            if (error.message.includes('404')) {
                showRegistrationForm(walletAddress);
            } else {
                showError(error.message);
            }
        }

    } catch (error) {
        showError(error.message);
    }
});