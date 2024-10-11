using Exchange.Rates.CoinCap.OpenApi.Models;
using Exchange.Rates.Contracts.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Exchange.Rates.CoinCap.OpenApi.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class ExchangeRatesCoinCapController(
  ILogger<ExchangeRatesCoinCapController> logger,
  IRequestClient<ISubmitCoinCapAssetId> submitCoinCapAssetIdRequestClient)
  : ControllerBase
{

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
        logger.LogError("Invalid Model");
        return BadRequest(ModelState);
      }

      // https://masstransit-project.com/usage/requests.html#request-client
      var (accepted, rejected) = await submitCoinCapAssetIdRequestClient.GetResponse<ICoinCapAssetAccepted, ICoinCapAssetRejected>(new
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
        logger.LogError(errMessage);
        return Problem(errMessage);
      }
      else
      {
        var response = await rejected.ConfigureAwait(false);
        logger.LogError(response.Message.Reason);
        return BadRequest(response.Message);
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, ex.Message);
      return BadRequest(ex.Message);
    }
  }
}
