using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Exchange.Rates.CoinCap.OpenApi.Filters
{
    /// <summary>
    /// Corresponding to Controller's API document description information
    /// </summary>
    public class SwaggerDocumentFilter : IDocumentFilter
    {
		private const string DOCS_URI = "https://docs.coincap.io/";

		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var tags = new List<OpenApiTag>
            {
                new()
                {
                    Name = "ExchangeRatesCoinCap",
                    Description = "CoinCap API 2.0",
                    ExternalDocs = new OpenApiExternalDocs
                    {
                        Description = "Read more",
                        Url = new Uri(DOCS_URI)
                    }
                }
            };

            // Sort in ascending order by AssemblyName
            swaggerDoc.Tags = tags.OrderBy(x => x.Name).ToList();
        }
    }
}