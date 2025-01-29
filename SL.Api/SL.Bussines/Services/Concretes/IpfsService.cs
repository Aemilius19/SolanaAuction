using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SL.Bussines.Services.Abstractions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SL.Core.DTO_s;

namespace SL.Bussines.Services.Concretes
{
    public class IpfsService : IIpfsService
    {
        private const string PinataApiKey = "91364a154964df40ea40"; // Замените на ваш API-ключ
        private const string PinataSecretKey = "731c72ff52c740e3e89a882fd62b67ad08e7a29c41d558a95e5e2364a80a6f95"; // Замените на ваш секретный ключ

        public async Task<string> UploadMetadataAsync(NftMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata), "Metadata cannot be null");

            if (string.IsNullOrWhiteSpace(metadata.Name) || string.IsNullOrWhiteSpace(metadata.Description))
                throw new ArgumentException("Metadata must contain a valid name and description.");

            using var httpClient = new HttpClient();

            // **Исправление: передача API-ключей в правильном формате**
            httpClient.DefaultRequestHeaders.Add("pinata_api_key", PinataApiKey);
            httpClient.DefaultRequestHeaders.Add("pinata_secret_api_key", PinataSecretKey);

            // Сериализуем метаданные в JSON
            var jsonData = JsonConvert.SerializeObject(new
            {
                pinataMetadata = new { name = metadata.Name },
                pinataContent = metadata
            });

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Отправляем запрос на Pinata
            var response = await httpClient.PostAsync("https://api.pinata.cloud/pinning/pinJSONToIPFS", content);

            // Проверяем успешность запроса
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to upload metadata to IPFS. Response: {responseContent}");
            }

            // Разбираем JSON-ответ
            var jsonResult = JObject.Parse(responseContent);

            if (!jsonResult.ContainsKey("IpfsHash"))
                throw new Exception("Response from Pinata does not contain 'IpfsHash'.");

            var ipfsHash = jsonResult["IpfsHash"]?.ToString();

            return $"ipfs://{ipfsHash}";
        }
    }
}
