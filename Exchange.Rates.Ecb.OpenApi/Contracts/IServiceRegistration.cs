using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exchange.Rates.Ecb.OpenApi.Contracts
{
    public interface IServiceRegistration
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}
