using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exchange.Rates.CoinCap.OpenApi.Contracts
{
    public interface IServiceRegistration
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}
