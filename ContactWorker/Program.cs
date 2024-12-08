using System.Reflection;
using Application.Services;
using ContactWorker;
using ContactWorker.Config;
using ContactWorker.Events;
using Domain.Abstractions;
using Infra;
using Infra.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var config = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .AddEnvironmentVariables()
    .Build();

var configuration = config.GetSection("RabbitMQConnection").Get<ConsumerConfiguration>();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.EnableSensitiveDataLogging()
        .UseNpgsql(config.GetConnectionString("PostgresConnectionString")));

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ConctactRepository>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(configuration.Host), h =>
        {
            h.Username(configuration.Username);
            h.Password(configuration.Password);
        });
        
        cfg.ReceiveEndpoint(configuration.ContactCreateQueue, e =>
        {
            e.ConfigureConsumer<CreateContactConsumer>(context);
        });
        
        cfg.ReceiveEndpoint(configuration.ContactUpdateQueue, e =>
        {
            e.ConfigureConsumer<UpdateContactConsumer>(context);
        });
        
        cfg.ReceiveEndpoint(configuration.ContactDeleteQueue, e =>
        {
            e.ConfigureConsumer<DeleteContactConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
