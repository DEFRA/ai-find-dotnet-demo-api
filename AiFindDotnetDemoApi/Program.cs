using AiFindDotnetDemoApi.Example.Endpoints;
using AiFindDotnetDemoApi.Example.Services;
using AiFindDotnetDemoApi.Utils;
using AiFindDotnetDemoApi.Utils.Http;
using AiFindDotnetDemoApi.Utils.Logging;
using AiFindDotnetDemoApi.Utils.Mongo;
using FluentValidation;
using Serilog;

namespace AiFindDotnetDemoApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        builder.Logging.ClearProviders();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.With<LogLevelMapper>()
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        logger.Information("Starting application");

        builder.Services.AddCustomTruststore(logger);

        builder.Services.AddSingleton<IMongoDbClientFactory>(_ =>
            new MongoDbClientFactory(builder.Configuration.GetValue<string>("Mongo:DatabaseUri")!,
                builder.Configuration.GetValue<string>("Mongo:DatabaseName")!));

        builder.Services.AddSingleton<IExamplePersistence, ExamplePersistence>();

        builder.Services.AddHealthChecks();
        builder.Services.AddHttpClient();
        builder.Services.AddHttpProxyClient(logger);
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var app = builder.Build();

        app.UseRouting();
        app.MapHealthChecks("/health");

        app.UseExampleEndpoints();

        app.Run();
    }
}