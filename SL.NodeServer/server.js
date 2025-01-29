import express from "express";
import cors from "cors";
import dotenv from "dotenv";
import bodyParser from "body-parser";
import { Connection, PublicKey, Transaction, clusterApiUrl, Keypair } from "@solana/web3.js";
import { Metaplex, keypairIdentity } from "@metaplex-foundation/js";

dotenv.config();

const app = express();
const PORT = process.env.PORT || 3001;

// Разрешаем CORS (чтобы фронт мог делать запросы)
app.use(cors());
app.use(bodyParser.json());

// Подключаемся к Solana Devnet
const connection = new Connection(clusterApiUrl("devnet"), "confirmed");

// Загружаем секретный ключ кошелька из .env
const secretKey = Uint8Array.from(JSON.parse(process.env.SOLANA_WALLET_SECRET || "[]"));
const walletKeypair = Keypair.fromSecretKey(secretKey);
const metaplex = Metaplex.make(connection).use(keypairIdentity(walletKeypair));

console.log("✅ Сервер Node.js запущен. Подключён к Solana Devnet.");

// 📌 **Эндпоинт для генерации минтинг-транзакции**
app.post("/api/nft/mint", async (req, res) => {
    try {
        const { userPublicKey, metadataUrl } = req.body;

        if (!userPublicKey || !metadataUrl) {
            return res.status(400).json({ error: "Отсутствуют параметры" });
        }

        console.log("📌 Минтинг NFT для пользователя:", userPublicKey);
        console.log("📌 Метаданные NFT:", metadataUrl);

        // Генерируем новый токен (NFT)
        const mintKeypair = Keypair.generate();
        const mintAddress = mintKeypair.publicKey;

        console.log("📌 Создан новый токен:", mintAddress.toBase58());

        // Создаём NFT через Metaplex SDK
        const { transaction } = await metaplex.nfts().builders().create({
            uri: metadataUrl, // URL метаданных, загруженных через API C#
            name: "Solana NFT",
            sellerFeeBasisPoints: 500, // 5% комиссия
            mint: mintAddress,
            owner: new PublicKey(userPublicKey),
        });

        // Отправляем подготовленную транзакцию на фронт (без подписи)
        const serializedTx = transaction.serialize().toString("base64");

        res.json({ transaction: serializedTx });
    } catch (error) {
        console.error("❌ Ошибка генерации минтинга:", error);
        res.status(500).json({ error: error.message });
    }
});

// 📌 **Эндпоинт для отправки подписанной транзакции**
app.post("/api/nft/send-transaction", async (req, res) => {
    try {
        const { serializedTx } = req.body;

        if (!serializedTx) {
            return res.status(400).json({ error: "Отсутствует транзакция" });
        }

        const txBuffer = Buffer.from(serializedTx, "base64");
        const transaction = Transaction.from(txBuffer);

        const txId = await connection.sendRawTransaction(transaction.serialize());

        console.log("✅ Транзакция отправлена:", txId);
        res.json({ txId });
    } catch (error) {
        console.error("❌ Ошибка отправки транзакции:", error);
        res.status(500).json({ error: error.message });
    }
});

// 🚀 Запускаем сервер
app.listen(PORT, () => {
    console.log(`🚀 Node.js сервер запущен на порту ${PORT}`);
});
