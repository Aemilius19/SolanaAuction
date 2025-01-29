using Microsoft.EntityFrameworkCore;
using SL.Application.Data.Context;
using SL.Application.Repositories.Abstractions;
using SL.Application.Repositories.Implementations;
using SL.Bussines.Services.Abstractions;
using SL.Bussines.Services.Concretes;
using SL.Core.Helper;

using Solnet.Rpc; // ��� IRpcClient � RpcClient
using Solnet.Rpc.Types;

namespace SL.Endpoints
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ��������� ����������� + Newtonsoft JSON (��� ��������� ReferenceLoop, ���� �����)
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // ���� ����������� OpenAPI/Swagger � ����������� (������� �� ����������)
            //builder.Services.AddOpenApi(); // ��������������, ��� � ��� ���� ���������� AddOpenApi()

            // ���������� DbContext
            builder.Services.AddDbContext<SlDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });

            // ��������� ��������� ��� OpenAI (������)
            var openAiSettings = builder.Configuration
                .GetSection("OpenAiSettings")
                .Get<OpenAiSettings>();
            builder.Services.AddSingleton(openAiSettings);



            // �����������
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
            builder.Services.AddScoped<INftRepository, NftRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // �������
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<INftService, NftService>();
            builder.Services.AddScoped<IIpfsService, IpfsService>();
            builder.Services.AddScoped<IBlockChainService, BlockChainService>();

            // ����������� CORS
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

            // ���������� Swagger / OpenAPI, ���� � ������ Development
            if (app.Environment.IsDevelopment())
            {
                // ��������������, ��� AddOpenApi() ��� ���������� MapOpenApi()
                app.MapOpenApi();
            }

            // ���������� CORS ��������
            app.UseCors("AllowAll");

            // �������������� HTTP -> HTTPS
            app.UseHttpsRedirection();

            // ���������� ����������� (���� �����)
            app.UseAuthorization();

            // ������ ����������� ����� (���� �����)
            app.UseStaticFiles();

            // ���������� �����������
            app.MapControllers();

            // ��������� ����������
            app.Run();
        }
    }
}
