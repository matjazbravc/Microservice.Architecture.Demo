using Exchange.Rates.Contracts.Messages;
using Exchange.Rates.CoinCap.OpenApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Exchange.Rates.CoinCap.OpenApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExchangeRatesCoinCapController : ControllerBase
    {
        private readonly ILogger<ExchangeRatesCoinCapController> _logger;
        private readonly IRequestClient<SubmitCoinCapAssetId> _submitCoinCapAssetIdRequestClient;

        public ExchangeRatesCoinCapController(ILogger<ExchangeRatesCoinCapController> logger,
            IRequestClient<SubmitCoinCapAssetId> submitCoinCapAssetIdRequestClient)
        {
            _logger = logger;
            _submitCoinCapAssetIdRequestClient = submitCoinCapAssetIdRequestClient;
        }

        /// <summary>
        /// Crypto AssetInfo endpoint
        /// </summary>
        /// <remarks>
        /// The asset price calculated by collecting ticker data from exchanges. Example Id: bitcoin
        /// </remarks>
        /// <param name="model">Unique identifier for asset. Ex: bitcoin</param>
        /// <returns></returns>
        /// <response code="200">Returned if everything is ok</response>
        /// <response code="400">Returned if something went wrong</response>
        [HttpGet("assetinfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssetInfo([FromQuery] AssetIdSubmissionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Model");
                    return BadRequest(ModelState);
                }

                // https://masstransit-project.com/usage/requests.html#request-client
                var (accepted, rejected) = await _submitCoinCapAssetIdRequestClient.GetResponse<CoinCapAssetAccepted, CoinCapAssetRejected>(new
                {
                    EventId = NewId.NextGuid(),
                    InVar.Timestamp,
                    model.Id
                }).ConfigureAwait(false);

                if (accepted.IsCompletedSuccessfully)
                {
                    var response = await accepted.ConfigureAwait(false);
                    return Ok(response.Message.AssetData);
                }

                if (accepted.IsCompleted)
                {
                    await accepted.ConfigureAwait(false);
                    var errMessage = "Asset Id was not accepted. Please check the syntax!";
                    _logger.LogError(errMessage);
                    return Problem(errMessage);
                }
                else
                {
                    var response = await rejected.ConfigureAwait(false);
                    _logger.LogError(response.Message.Reason);
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
