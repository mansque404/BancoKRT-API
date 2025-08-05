using Amazon.DynamoDBv2;
using BancoKRT.Application.Interfaces.Repositories;
using BancoKRT.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BancoKRT.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());

            services.AddAWSService<IAmazonDynamoDB>();

            services.AddScoped<IClientePixRepository, ClientePixRepository>();

            return services;
        }
    }
}