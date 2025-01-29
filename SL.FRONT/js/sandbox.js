const API_BASE_URL = 'https://localhost:7194';

document.addEventListener('DOMContentLoaded', async () => {
    const generateBtn = document.getElementById('generateBtn');
    const saveBtn = document.getElementById('saveBtn');
    const promptInput = document.getElementById('promptInput');
    const charCount = document.getElementById('charCount');
    const connectWalletBtn = document.getElementById('connectWallet');
    
    let currentUserId = '';
    let generatedImageUrl = '';
    

    // Функция подключения кошелька
    async function connectWallet() {
        try {
            const provider = window.phantom?.solana;
            if (!provider?.isPhantom) {
                showStatus('Установите Phantom Wallet', 'error');
                window.open('https://phantom.app/', '_blank');
                return false;
            }

            const { publicKey } = await provider.connect();
            const walletAddress = publicKey.toString();
            
            // Получаем данные пользователя с сервера
            const userData = await fetchUserData(walletAddress);
            currentUserId = userData.id; // Используем серверный ID пользователя
            
            const shortAddress = `${walletAddress.slice(0, 6)}...${walletAddress.slice(-4)}`;
            connectWalletBtn.textContent = `✔️ ${shortAddress}`;
            
            return true;
            
        } catch (error) {
            if (error.message.includes('404')) {
                showRegistrationForm(walletAddress);
            } else {
                showStatus(`Ошибка подключения: ${error.message}`, 'error');
            }
            return false;
        }
    }

    // Автоподключение при загрузке
    try {
        if (!await connectWallet()) return;
    } catch (error) {
        showStatus(error.message, 'error');
        return;
    }

    // Ручное подключение по кнопке
    connectWalletBtn.addEventListener('click', async () => {
        await connectWallet();
    });

    // Обработчик ввода текста
    promptInput.addEventListener('input', updateCharCounter);

    // Генерация изображения
    generateBtn.addEventListener('click', async () => {
        if (!currentUserId) {
            showStatus('Сначала подключите кошелек', 'error');
            return;
        }
    
        const prompt = promptInput.value.trim();
    
        if (!prompt) {
            showStatus('Введите описание для генерации', 'error');
            return;
        }
    
        try {
            showStatus('Генерация изображения...');
            generateBtn.disabled = true;
            showLoading('Генерация изображения...'); // Указан текст
    
            const response = await fetch(`${API_BASE_URL}/api/image/generate`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    prompt,
                    userId: currentUserId,
                }),
            });
    
            const data = await response.json();
    
            if (!response.ok) {
                throw new Error(data.error || data.Error || 'Ошибка генерации');
            }
    
            generatedImageUrl = data.imageUrl || data.ImageUrl;
    
            const img = new Image();
            img.src = generatedImageUrl;
    
            img.onload = () => {
                document.getElementById('generatedImage').src = generatedImageUrl;
                document.getElementById('resultSection').classList.remove('hidden');
                hideLoading(); // Скрываем индикатор после загрузки
                generateBtn.disabled = false;
                showStatus('Изображение успешно сгенерировано!', 'success');
            };
    
            img.onerror = () => {
                hideLoading();
                generateBtn.disabled = false;
                throw new Error('Ошибка загрузки изображения');
            };
        } catch (error) {
            hideLoading();
            generateBtn.disabled = false;
            showStatus(`Ошибка: ${error.message}`, 'error');
        }
    });
    

    // Добавляем новый обработчик для показа формы
document.getElementById('showSaveFormBtn').addEventListener('click', () => {
    document.getElementById('saveFormContainer').classList.remove('hidden');
    document.getElementById('showSaveFormBtn').style.display = 'none';
});

    // Сохранение изображения
    saveBtn.addEventListener('click', async () => {
        try {
            if (!currentUserId) throw new Error('Требуется авторизация');
            if (!generatedImageUrl) throw new Error('Сначала сгенерируйте изображение');
    
            saveBtn.disabled = true;
            showLoading('Сохранение изображения...'); // Указан текст
    
            const name = document.getElementById('imageName').value.trim() || 'Без названия';
            const description = document.getElementById('imageDescription').value.trim() || 'Без описания';
    
            showStatus('Сохранение изображения...');
            const response = await fetch(`${API_BASE_URL}/api/image/save`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    ImageUrl: generatedImageUrl, // PascalCase!
                    UserId: currentUserId,        // PascalCase!
                    Name: name,
                    Description: description,
                }),
            });
    
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.title || errorData.Error || 'Ошибка сохранения');
            }
    
            showStatus('Изображение успешно сохранено!', 'success');
    
            setTimeout(() => {
                hideLoading(); // Скрываем индикатор
                window.location.href = 'gallery.html';
            }, 2000);
        } catch (error) {
            hideLoading();
            showStatus(`Ошибка сохранения: ${error.message}`, 'error');
        } finally {
            saveBtn.disabled = false;
        }
    });
    

    function updateCharCounter() {
        charCount.textContent = promptInput.value.length;
    }

    function showStatus(message, type = 'info') {
        const statusElement = document.getElementById('statusMessage');
        statusElement.textContent = message;
        statusElement.className = `status-message ${type}`;
        statusElement.classList.remove('hidden');
        
        if (type !== 'error') {
            setTimeout(() => {
                statusElement.classList.add('hidden');
            }, 5000);
        }
    }
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
    function showLoading(message = 'Загрузка...') {
        console.log(`Показываем спиннер с сообщением: ${message}`);
        const loadingSpinner = document.getElementById('loadingSpinner');
        const spinnerText = loadingSpinner.querySelector('p'); // Находим текст в спиннере
        if (loadingSpinner) {
            loadingSpinner.classList.remove('hidden');
            if (spinnerText) {
                spinnerText.textContent = message; // Меняем текст на указанный
            }
        } else {
            console.error('Спиннер не найден в DOM');
        }
    }
    
    function hideLoading() {
        const loadingSpinner = document.getElementById('loadingSpinner');
        if (loadingSpinner) {
            loadingSpinner.classList.add('hidden');
        }
    }
});