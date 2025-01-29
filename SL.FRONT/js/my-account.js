const API_BASE_URL = 'https://localhost:7194';

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
async function fetchUserData(walletAddress) {
    showLoading(); // Показать индикатор загрузки
    try {
        const response = await fetch(
            `${API_BASE_URL}/api/user/${encodeURIComponent(walletAddress)}`,
            { headers: { 'Accept': 'application/json' } }
        );

        if (response.status === 404) {
            throw new Error('404: Пользователь не найден');
        }

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.Error || 'Ошибка сервера');
        }

        return await response.json();
    } finally {
        hideLoading(); // Скрыть индикатор загрузки
    }
}
async function registerUser(walletAddress, username) {
    showLoading(); // Показать индикатор загрузки
    try {
        const response = await fetch(`${API_BASE_URL}/api/user/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ walletAddress, username })
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.Error || 'Ошибка регистрации');
        }

        return await response.json();
    } finally {
        hideLoading(); // Скрыть индикатор загрузки
    }
}

function showLoading() {
    const loadingSpinner = document.getElementById('loadingSpinner');
    if (loadingSpinner) {
        loadingSpinner.classList.remove('hidden');
    }
}

function hideLoading() {
    const loadingSpinner = document.getElementById('loadingSpinner');
    if (loadingSpinner) {
        loadingSpinner.classList.add('hidden');
    }
}

function showUserProfile(user, walletAddress) {
    const profileHTML = `
        <div class="profile-card">
            <div class="profile-header">
                <img src="/images/account.svg" 
                     alt="Иконка аккаунта" 
                     class="account-icon">
                <h2>${user.username}</h2>
                <button id="changeUsernameBtn" class="edit-btn">
                    <img src="/images/edit.svg" 
                         alt="Изменить" 
                         class="edit-icon">
                    Изменить Никнейм
                </button>
            </div>
            <div class="profile-details">
                <p><span>Адрес кошелька:</span> <code>${walletAddress}</code></p>
                <p><span>ID пользователя:</span> <code>${user.id}</code></p>
                <p><span>NFT:</span> ${user.nfts?.length || 0}</p>
                <p><span>Изображений:</span> ${user.images?.length || 0}</p>
            </div>
            <div id="usernameForm" class="hidden">
                <input type="text" id="newUsername" placeholder="Новый никнейм">
                <button id="saveUsernameBtn" class="btn-primary">Сохранить</button>
                <button id="cancelBtn" class="btn-secondary">Отмена</button>
            </div>
        </div>
    `;
    document.getElementById('userInfo').innerHTML = profileHTML;
    initUsernameChange(walletAddress); // Исправлено: передаем walletAddress
}

// Инициализация смены ника
function initUsernameChange(walletAddress) {
    const changeBtn = document.getElementById('changeUsernameBtn');
    const saveBtn = document.getElementById('saveUsernameBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const form = document.getElementById('usernameForm');
    const input = document.getElementById('newUsername');

    if (!changeBtn || !saveBtn || !cancelBtn || !form || !input) {
        console.error('Один или несколько элементов DOM не найдены');
        return;
    }

    changeBtn.addEventListener('click', () => {
        form.classList.remove('hidden');
    });

    cancelBtn.addEventListener('click', () => {
        form.classList.add('hidden');
        input.value = '';
    });

    saveBtn.addEventListener('click', async () => {
        const newUsername = input.value.trim();
        
        if (!newUsername) {
            showError('Введите новый никнейм');
            return;
        }

        try {
            // 1. Изменение никнейма
            await changeUsername(walletAddress, newUsername);

            // 2. Получение обновленных данных пользователя
            const updatedUser = await fetchUserData(walletAddress);

            // 3. Отображение обновленного профиля
            showUserProfile(updatedUser, walletAddress);

            // 4. Скрытие формы после успешного изменения
            form.classList.add('hidden');
            input.value = '';
        } catch (error) {
            showError(error.message);
        }
    });
}

// Смена никнейма через API
async function changeUsername(walletAddress, newUsername) {
    if (!newUsername) {
        throw new Error('Новый никнейм не может быть пустым');
    }

    showLoading(); // Показать индикатор загрузки
    try {
        const response = await fetch(`${API_BASE_URL}/api/user/change-username`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                NewUsername: newUsername,
                WalletAddress: walletAddress
            })
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.errors?.NewUsername?.[0] || 'Ошибка изменения ника');
        }

        return await response.json();
    } finally {
        hideLoading(); // Скрыть индикатор загрузки
    }
}
// Форма регистрации
function showRegistrationForm(walletAddress) {
    const form = document.getElementById('registrationForm');
    const usernameInput = document.getElementById('usernameInput');
    const registerBtn = document.getElementById('registerBtn');

    if (!form || !usernameInput || !registerBtn) {
        console.error('Элементы формы регистрации не найдены');
        return;
    }

    form.style.display = 'block';
    registerBtn.disabled = false;

    registerBtn.onclick = async () => {
        const username = usernameInput.value.trim();
        
        if (!username) {
            showError('Введите имя пользователя');
            return;
        }

        try {
            const newUser = await registerUser(walletAddress, username);
            showUserProfile(newUser, walletAddress);
            form.style.display = 'none';
        } catch (error) {
            showError(error.message);
        }
    };
}

// Вспомогательные функции
function showError(message) {
    const errorElement = document.getElementById('errorMessage');
    if (errorElement) {
        errorElement.textContent = `⚠️ ${message}`;
        errorElement.style.display = 'block';
        setTimeout(() => errorElement.style.display = 'none', 5000);
    }
}

// Инициализация
function init() {
    const registrationForm = document.getElementById('registrationForm');
    const errorMessage = document.getElementById('errorMessage');

    if (registrationForm) registrationForm.style.display = 'none';
    if (errorMessage) errorMessage.style.display = 'none';
}

init();