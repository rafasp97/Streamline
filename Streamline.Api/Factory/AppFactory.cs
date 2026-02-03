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

            var sqlConnection = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION");
            var mongoConnection = Environment.GetEnvironmentVariable("MONGO_CONNECTION");
            var mongoDatabase = Environment.GetEnvironmentVariable("MONGO_DATABASE");
            var rabbitConnection = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION");

            if (
                string.IsNullOrEmpty(mongoConnection) || 
                string.IsNullOrEmpty(mongoDatabase) ||
                string.IsNullOrEmpty(sqlConnection) ||
                string.IsNullOrEmpty(rabbitConnection))
            {
                throw new InvalidOperationException("Variáveis de ambiente não definidas.");
            }

            builder.Services.AddDbContext<SqlServerDbContext>(options =>
                options.UseSqlServer(sqlConnection));

            builder.Services.AddSingleton(new MongoDbContext(mongoConnection, mongoDatabase));
            builder.Services.AddSingleton(new RabbitMqSettings(rabbitConnection));
            builder.Services.AddSingleton<RabbitMqPublisher>();
            builder.Services.AddSingleton<RabbitMqConsumer>();
            builder.Services.AddSingleton<IOrderProcessingQueue, OrderProcessingQueue>();
            builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

            builder.Services.AddSingleton<IConnection>(sp =>
            {
                var settings = sp.GetRequiredService<RabbitMqSettings>();
                var factory = new RabbitMQ.Client.ConnectionFactory() 
                { 
                    Uri = new Uri(settings.ConnectionString) 
                };
                return factory.CreateConnection();
            });

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ILogRepository, LogRepository>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<LogRepository>();
            builder.Services.AddScoped<OrderProcessingWorker>();

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            });

            builder.Services.AddHostedService(provider => (OrderProcessingQueue)provider.GetRequiredService<IOrderProcessingQueue>());
            builder.Services.AddHostedService<RabbitMqConsumerQueue>();


            builder.Services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });
            builder.Services.AddTransient<RetryFaildedOrdersWorker>();
            builder.Services.AddHangfireServer();

            builder.Services.AddAutoMapper(typeof(AppFactory));

            builder.Services.AddMvc();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHostedService<OrderProcessingQueue>();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Streamline E-commerce", Version = "v1" });
                options.EnableAnnotations();
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                HangfireSettings.ConfigureJobs(recurringJobManager);
            }

            app.MapSwagger();

            app.UseSwagger();
            
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "Streamline E-commerce");
            });

            return app;
        }
    }
}
