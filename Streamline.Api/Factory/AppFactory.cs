using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Application.Interfaces.Queues;
using Streamline.Application.Commands;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Infrastructure.Persistence.SqlServer.Repositories;
using Streamline.Infrastructure.Persistence.MongoDb.DbContexts;
using Streamline.Infrastructure.Persistence.MongoDb.Repositories;
using Streamline.Infrastructure.Messaging.RabbitMq;
using Streamline.Infrastructure.Queues;
using Streamline.Infrastructure.BackgroundWorkers.Workers;
using RabbitMQ.Client;
using Streamline.Infrastructure.BackgroundWorkers.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;

namespace Streamline.API.Factory
{
    public static class AppFactory
    {
        public static WebApplication CreateApp(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            var sqlConnection = GetEnv("SQLSERVER_CONNECTION");
            var mongoConnection = GetEnv("MONGO_CONNECTION");
            var mongoDatabase = GetEnv("MONGO_DATABASE");
            var rabbitConnection = GetEnv("RABBITMQ_CONNECTION");
            
            // Configurações de Banco de Dados
            builder.Services.AddDbContext<SqlServerDbContext>(o => o.UseSqlServer(sqlConnection));
            builder.Services.AddSingleton(new MongoDbContext(mongoConnection, mongoDatabase));

            // Configurações RabbitMQ
            builder.Services.AddSingleton(new RabbitMqSettings(rabbitConnection));
            builder.Services.AddSingleton<RabbitMqPublisher>();
            builder.Services.AddSingleton<RabbitMqConsumer>();
            builder.Services.AddSingleton<IOrderProcessingQueue, OrderProcessingQueue>();
            builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());
            builder.Services.AddSingleton<IConnection>(sp =>
            {
                var settings = sp.GetRequiredService<RabbitMqSettings>();
                return new ConnectionFactory { Uri = new Uri(settings.ConnectionString) }.CreateConnection();
            });

            // Repositórios
            AddScopedServices(builder, new (Type service, Type implementation)[]
            {
                (typeof(ICustomerRepository), typeof(CustomerRepository)),
                (typeof(IProductRepository), typeof(ProductRepository)),
                (typeof(IOrderRepository), typeof(OrderRepository)),
                (typeof(ILogRepository), typeof(LogRepository)),
                (typeof(OrderRepository), typeof(OrderRepository)),
                (typeof(LogRepository), typeof(LogRepository)),
                (typeof(OrderProcessingWorker), typeof(OrderProcessingWorker))
            });

            // MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            // Hosted Services
            builder.Services.AddHostedService(provider => (OrderProcessingQueue)provider.GetRequiredService<IOrderProcessingQueue>());
            builder.Services.AddHostedService<RabbitMqConsumerQueue>();
            builder.Services.AddHostedService<OrderProcessingQueue>();

            // Hangfire
            builder.Services.AddHangfire(cfg => cfg.UseMemoryStorage());
            builder.Services.AddHangfireServer();
            builder.Services.AddTransient<RetryFaildedOrdersWorker>();

             // AutoMapper
            builder.Services.AddAutoMapper(typeof(AppFactory));

            // MVC & Swagger
            builder.Services.AddMvc();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Streamline E-commerce", Version = "v1" });
                options.EnableAnnotations();
            });

            var app = builder.Build();

            // Configura Jobs do Hangfire
            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                HangfireSettings.ConfigureJobs(recurringJobManager);
            }

            app.MapSwagger();

            app.UseSwagger();
            app.UseSwaggerUI(cfg => cfg.SwaggerEndpoint("v1/swagger.json", "Streamline E-commerce"));


            return app;
        }

        private static string GetEnv(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"Variável de ambiente '{key}' não definida.");
            return value;
        }

        private static void AddScopedServices(WebApplicationBuilder builder, (Type service, Type implementation)[] services)
        {
            foreach (var (service, implementation) in services)
                builder.Services.AddScoped(service, implementation);
        }
    }
}
