using Exchange.Rates.Contracts.Models;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.Polling.Api.Services
{
    public interface ICoinCapAssetsApi
    {
        Task<CoinCapAsset> GetAssetData(string id);
    }
}