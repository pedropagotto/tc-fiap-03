using MassTransit;

namespace API.Config;

public static class MassTransitConfig
{
    public static WebApplicationBuilder AddMassTransitConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                var host = builder.Configuration["RabbitMQConnection:Host"]!;
                var username = builder.Configuration["RabbitMQConnection:Username"]!;
                var password = builder.Configuration["RabbitMQConnection:Password"]!;

                cfg.Host(new Uri(host), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
                
                cfg.ConfigureEndpoints(ctx);
            });
        });
        
        return builder; 
    }
}