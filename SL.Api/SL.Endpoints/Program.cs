using Microsoft.EntityFrameworkCore;
using SL.Application.Data.Context;
using SL.Application.Repositories.Abstractions;
using SL.Application.Repositories.Implementations;
using SL.Bussines.Services.Abstractions;
using SL.Bussines.Services.Concretes;
using SL.Core.Helper;

using Solnet.Rpc; // Для IRpcClient и RpcClient
using Solnet.Rpc.Types;

namespace SL.Endpoints
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем контроллеры + Newtonsoft JSON (для обработки ReferenceLoop, если нужно)
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // Если используете OpenAPI/Swagger — настраиваем (зависит от библиотеки)
            //builder.Services.AddOpenApi(); // Предполагается, что у вас есть расширение AddOpenApi()

            // Подключаем DbContext
            builder.Services.AddDbContext<SlDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });

            // Загружаем настройки для OpenAI (пример)
            var openAiSettings = builder.Configuration
                .GetSection("OpenAiSettings")
                .Get<OpenAiSettings>();
            builder.Services.AddSingleton(openAiSettings);



            // Репозитории
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
            builder.Services.AddScoped<INftRepository, NftRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Сервисы
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<INftService, NftService>();
            builder.Services.AddScoped<IIpfsService, IpfsService>();
            builder.Services.AddScoped<IBlockChainService, BlockChainService>();

            // Настраиваем CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Подключаем Swagger / OpenAPI, если в режиме Development
            if (app.Environment.IsDevelopment())
            {
                // Предполагается, что AddOpenApi() даёт расширение MapOpenApi()
                app.MapOpenApi();
            }

            // Подключаем CORS политику
            app.UseCors("AllowAll");

            // Перенаправляем HTTP -> HTTPS
            app.UseHttpsRedirection();

            // Подключаем авторизацию (если нужна)
            app.UseAuthorization();

            // Раздаём статические файлы (если нужно)
            app.UseStaticFiles();

            // Подключаем контроллеры
            app.MapControllers();

            // Запускаем приложение
            app.Run();
        }
    }
}
