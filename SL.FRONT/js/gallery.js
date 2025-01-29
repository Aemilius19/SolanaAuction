import { Buffer } from 'buffer';

if (!window.Buffer) {
  window.Buffer = Buffer;
}

import {
    Connection,
    PublicKey,
    Transaction,
    clusterApiUrl
} from "@solana/web3.js";

const API_BASE_URL = "https://localhost:7194"; // .NET сервер (API)
const NODE_SERVER_URL = "http://localhost:3001"; // Node.js сервер
let currentUserId = null;


document.addEventListener('DOMContentLoaded', async () => {
    await initWallet();
    if (currentUserId) {
        loadGallery();
        
    }
});
async function initWallet() {
    const provider = window.phantom?.solana;
    if (!provider?.isPhantom) {
        showError('Требуется установка Phantom Wallet');
        return;
    }

    try {
        const { publicKey } = await provider.connect();
        const walletAddress = publicKey.toString();
        const shortAddress = `${publicKey.toString().slice(0, 6)}...${publicKey.toString().slice(-4)}`;
        document.getElementById("connectWallet").textContent = `✔️ ${shortAddress}`;
        
        const userData = await fetchUserData(walletAddress);
        if (userData && userData.id) {
            currentUserId = userData.id;
            updateUserInfo(userData); // Добавлен вызов функции обновления информации
            // Обновляем кнопку
            loadGallery();
        } else {
            showError('Пользователь не найден');
        }
    } catch (error) {
        showError('Ошибка подключения кошелька: ' + error.message);
    }
}
function updateUserInfo(userData) {
    document.querySelector('.user-nickname').textContent = userData.username;
    document.querySelector('.images-count').textContent = `Изображений: ${userData.imagesCount || 0}`;
}

async function fetchUserData(walletAddress) {
    try {
        const response = await fetch(
            `${API_BASE_URL}/api/user/${encodeURIComponent(walletAddress)}`,
            { headers: { 'Accept': 'application/json' } }
        );
        return await handleJSONResponse(response);
    } catch (error) {
        console.error('Ошибка:', error);
        throw error;
    }
}

document.getElementById('uploadBtn').addEventListener('click', () => {
    document.getElementById('fileInput').click();
});

document.getElementById('fileInput').addEventListener('change', async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
        document.getElementById('previewImage').src = reader.result; // Устанавливаем превью
        openUploadModal();
    };
    reader.readAsDataURL(file);

    document.getElementById('confirmUpload').onclick = async () => {
        await uploadImage(file);
        closeUploadModal();
    };

    document.getElementById('cancelUpload').onclick = () => {
        closeUploadModal();
    };
});

// Обработчик «Минт NFT» (ключевое место)
async function handleMintImage(event) {
    try {
        event.preventDefault();

        const btn = event.target;
        const imageId = btn.dataset.imageId;
        const imageName = btn.dataset.name;
        const imageDescription = btn.dataset.description || "No description";
        const imageUrl = btn.dataset.url || "";

        // Проверим, что Phantom подключён:
        const provider = window.phantom?.solana;
        if (!provider?.isConnected || !currentUserId) {
            throw new Error("Phantom кошелёк не подключён");
        }

        // 📌 1) Загружаем метаданные в IPFS через API
        const metadata = { name: imageName, description: imageDescription, image: imageUrl };
        const metadataResp = await fetch(`${API_BASE_URL}/api/nft/upload-metadata`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(metadata),
        });

        if (!metadataResp.ok) throw new Error("Ошибка загрузки метаданных");
        const { ipfsUrl } = await metadataResp.json();
        console.log("✅ Метаданные загружены:", ipfsUrl);

        // 📌 2) Запрашиваем у Node.js сервера минтинг-транзакцию
        const userPubkey = provider.publicKey.toString();
        const mintResp = await fetch(`${NODE_SERVER_URL}/api/nft/mint`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userPublicKey: userPubkey, metadataUrl: ipfsUrl }),
        });

        if (!mintResp.ok) throw new Error("Ошибка генерации транзакции");
        const { transaction: base64Tx } = await mintResp.json();
        if (!base64Tx) throw new Error("Сервер не вернул транзакцию");

        // 📌 3) Подписываем транзакцию в Phantom
        const connection = new Connection(clusterApiUrl("devnet"), "confirmed");
        const txBuffer = Buffer.from(base64Tx, "base64");
        let transaction = Transaction.from(txBuffer);
        transaction = await provider.signTransaction(transaction);

        // 📌 4) Отправляем подписанную транзакцию обратно на сервер
        const sendResp = await fetch(`${NODE_SERVER_URL}/api/nft/send-transaction`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ serializedTx: transaction.serialize().toString("base64") }),
        });

        if (!sendResp.ok) throw new Error("Ошибка отправки в сеть");
        const { txId } = await sendResp.json();
        console.log("✅ NFT заминчен! TX ID:", txId);

        // 📌 5) (Опционально) Показываем успех, обновляем галерею
        alert(`NFT успешно заминтирован!\nTx: ${txId}`);
        loadGallery();

    } catch (error) {
        console.error("❌ Ошибка минтинга:", error);
        showError(error.message || "Ошибка при минтинге NFT");
    }
}

// Новая функция для обновления информации
function updateUserHeader(userData) {
    const header = document.createElement('div');
    header.className = 'user-info-header';
    header.innerHTML = `
        <div class="user-stats">
            <span class="username">${userData.username}</span>
            <span class="image-count">Изображений: ${userData.images?.length || 0}</span>
        </div>
    `;
    
    const container = document.querySelector('.container');
    container.insertBefore(header, container.firstChild);
}


// Обновлённая функция для загрузки изображения


async function uploadImage(file) {
    if (!currentUserId) {
        showError('Пользователь не идентифицирован');
        return;
    }

    const formData = new FormData();
    formData.append('image', file);
    formData.append('userId', currentUserId);
    formData.append('name', document.getElementById('modalImageName').value.trim());
    formData.append('description', document.getElementById('modalImageDesc').value.trim());

    try {
        const response = await fetch(`${API_BASE_URL}/api/image/upload`, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Ошибка загрузки изображения');
        }

        const data = await response.json();
        console.log('Изображение успешно загружено:', data);
        loadGallery();
    } catch (error) {
        showError(`Ошибка: ${error.message}`);
    }
}

function handleJSONResponse(response) {
    return response.text().then(text => {
        if (!response.ok) {
            throw new Error(text || `HTTP Error ${response.status}`);
        }
        try {
            return text ? JSON.parse(text) : [];
        } catch (error) {
            throw new Error('Ошибка парсинга JSON');
        }
    });
}

// Исправление URL эндпоинта в клиентском коде
// Модифицированная функция loadGallery
// В функции loadGallery
async function loadGallery() {
    if (!currentUserId) {
        showError('Пользователь не идентифицирован');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/image/${currentUserId}`);
        
        console.log('Статус ответа:', response.status);
        const responseText = await response.text();
        console.log('Сырой ответ:', responseText);

        if (!response.ok) {
            throw new Error(`HTTP Error ${response.status}`);
        }

        const images = JSON.parse(responseText);
        renderGallery(images);
        

    } catch (error) {
        showError('Ошибка загрузки: ' + error.message);
    }
}

// Новая функция для обновления счетчика
// Обновленная функция рендеринга галереи
function renderGallery(images) {
    const container = document.getElementById('galleryContainer');
    container.innerHTML = images.map(img => `
        <div class="gallery-card">
            <img src="${API_BASE_URL}${img.url}" 
                 alt="${img.name}" 
                 class="gallery-image"
                 data-name="${img.name}"
                 data-description="${img.description || 'Нет описания'}"
                 data-date="${new Date(img.createdAt).toLocaleDateString()}"
                 data-views="${img.views || 0}">
            <div class="image-meta">
                <h4>${img.name}</h4>
                <p>${img.description}</p>
                <div class="image-actions">
                    <button class="btn-delete" data-image-id="${img.id}">🗑️ Удалить</button>
                     <button class="btn-mint" 
                            data-image-id="${img.id}"
                            data-name="${img.name}"
                            data-description="${img.description || 'Нет описания'}"
                            data-url="${API_BASE_URL}${img.url}"
                            ${img.isMinted ? 'disabled' : ''}>
                        ${img.isMinted ? '✅ Уже минтировано' : '🖼️ Минтить NFT'}
                    </button>
                </div>
                ${img.isMinted ? '<span class="nft-badge">NFT</span>' : ''}
            </div>
        </div>
    `).join('');

    // Добавленные обработчики
    document.querySelectorAll('.gallery-image').forEach(img => {
        img.addEventListener('click', () => {
            openImageDetails({
                url: img.src,
                name: img.dataset.name,
                description: img.dataset.description,
                date: img.dataset.date,
                views: img.dataset.views
            });
        });
    });

    document.querySelectorAll('.btn-delete').forEach(btn => {
        btn.addEventListener('click', handleDeleteImage);
    });
    
    document.querySelectorAll('.btn-mint').forEach(btn => {
        btn.addEventListener('click', handleMintImage);
    });
}
async function handleDeleteImage(event) {
    const imageId = event.target.dataset.imageId;
    
    // Проверка авторизации
    if (!currentUserId) {
        showError('Требуется авторизация');
        return;
    }

    // Подтверждение действия
    if (!confirm('Вы уверены, что хотите удалить это изображение?')) {
        return;
    }

    try {
        const response = await fetch(
            `${API_BASE_URL}/api/image/${encodeURIComponent(imageId)}?userId=${encodeURIComponent(currentUserId)}`, 
            {
                method: 'DELETE',
                headers: {
                    'Accept': 'application/json'
                }
            }
        );

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.Error || 'Ошибка удаления');
        }

        // Обновляем галерею после успешного удаления
        loadGallery();
        showError('Изображение успешно удалено', 'success');

    } catch (error) {
        showError(error.message);
        console.error('Delete error:', error);
    }
}



// Вспомогательные функции
function showForm() {
    const form = document.getElementById('imageForm');
    form.classList.remove('hidden');
    form.style.opacity = '1';
    form.style.transform = 'translateY(0)';
}

function closeForm() {
    const form = document.getElementById('imageForm');
    form.style.opacity = '0';
    form.style.transform = 'translateY(20px)';
    setTimeout(() => {
        form.classList.add('hidden');
        document.getElementById('fileInput').value = '';
        document.getElementById('imageName').value = '';
        document.getElementById('imageDesc').value = '';
    }, 300);
}

function showError(message) {
    const errorElement = document.getElementById('errorMessage');
    errorElement.textContent = message;
    errorElement.classList.remove('hidden');
    setTimeout(() => errorElement.classList.add('hidden'), 5000);
}

function shortenAddress(address, start = 6, end = 4) {
    return `${address.slice(0, start)}...${address.slice(-end)}`;
}

// Обработчики для модального окна
document.getElementById('galleryContainer').addEventListener('click', function(e) {
    const target = e.target;
    if (target.classList.contains('gallery-image')) {
        openModal(target.src);
    }
});

document.getElementById('imageDetailsModal').addEventListener('click', function(e) {
    if(e.target === this || e.target.classList.contains('close-modal')) {
        this.classList.add('hidden');
    }
});

function openModal(imageUrl) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('modalImage');
    if (!modal || !modalImg) return;

    modalImg.src = imageUrl;
    modal.classList.remove('hidden');
}

function closeModal() {
    document.getElementById('imageModal').classList.add('hidden');
}

// Открытие модального окна
function openUploadModal() {
    const modal = document.getElementById('uploadModal');
    modal.classList.remove('hidden');
}

// Закрытие модального окна
function closeUploadModal() {
    const modal = document.getElementById('uploadModal');
    modal.classList.add('hidden');
    // Очистка полей
    document.getElementById('modalImageName').value = '';
    document.getElementById('modalImageDesc').value = '';
    document.getElementById('fileInput').value = '';
}

function openImageDetails(data) {
    const modal = document.getElementById('imageDetailsModal');
    if (!modal) return;

    const elements = {
        image: modal.querySelector('#detailsImage'),
        title: modal.querySelector('#detailsTitle'),
        desc: modal.querySelector('#detailsDescription'),
        date: modal.querySelector('#detailsDate'),
        views: modal.querySelector('#detailsViews')
    };

    if (Object.values(elements).some(el => !el)) return;

    elements.image.src = data.url;
    elements.title.textContent = data.name;
    elements.desc.textContent = data.description;
    elements.date.textContent = `Дата: ${data.date}`;
    elements.views.textContent = `Просмотров: ${data.views}`;
    
    modal.classList.remove('hidden');
}

document.addEventListener('DOMContentLoaded', () => {
    const modal = document.querySelector('.modal-content');
    const textarea = document.querySelector('textarea.form-input');

    // Обработчик изменения высоты textarea
    textarea.addEventListener('input', () => {
        adjustModalHeight();
    });
    

    function adjustModalHeight() {
        const textareaHeight = textarea.scrollHeight; // Текущая высота textarea
        const modalPadding = 40; // Учитываем padding модального окна (20px сверху и снизу)
        modal.style.height = `${textareaHeight + modalPadding}px`; // Устанавливаем новую высоту
    }

    
});

// Отдельная функция для загрузки метаданных
async function uploadMetadataToIpfs(metadata) {
    try {
        console.log("Отправляем метаданные в IPFS:", metadata);

        const response = await fetch(`${API_BASE_URL}/api/nft/upload-metadata`, {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(metadata),
        });

        console.log("Ответ от сервера:", response);

        const responseText = await response.text();
        console.log("Сырой JSON-ответ:", responseText);

        const data = JSON.parse(responseText);
        console.log("Разобранный JSON:", data);

        // 🔹 Исправляем ошибку с регистром букв
        if (!data.ipfsUrl) {
            throw new Error("Ошибка: В ответе нет 'ipfsUrl'");
        }

        return data.ipfsUrl;
    } catch (error) {
        console.error('Ошибка при загрузке метаданных в IPFS:', error);
        throw error;
    }
}
async function checkWalletBalance(connection, publicKey, minBalance = 0.01) {
    try {
        const balance = await connection.getBalance(publicKey);
        const solBalance = balance / 10**9; // Конвертация lamports в SOL
        return solBalance >= minBalance;
    } catch (error) {
        console.error('Balance check failed:', error);
        return false;
    }
}


// Добавьте этот код вместо удаленных строк
document.querySelectorAll('.modal').forEach(modal => {
    // Закрытие по клику на крестик
    modal.querySelector('.close-modal')?.addEventListener('click', () => {
        modal.classList.add('hidden');
    });
    
    // Закрытие по клику на фон
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.classList.add('hidden');
        }
    });
});