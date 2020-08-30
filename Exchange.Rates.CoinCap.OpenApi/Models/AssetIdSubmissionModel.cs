using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Exchange.Rates.CoinCap.OpenApi.Models
{
    public sealed class AssetIdSubmissionModel
    {
        /// <summary>
        /// Unique identifier for asset. Ex: bitcoin
        /// </summary>
        [Required]
        [FromQuery(Name = "Id")]
        public string Id { get; set; }
    }
}
