using DotNetEnv;
using CoffeShop.Infrastructure.Persistence.SqlServer.DbContexts;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var SQLSERVER_CONNECTION = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION");

builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(SQLSERVER_CONNECTION));

var app = builder.Build();

app.MapGet("/", () => "API rodando!");

app.MapGet("/test-sql", async (SqlServerDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok("Conexão com SQL Server OK!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao conectar: {ex.Message}");
    }
});


app.Run();
