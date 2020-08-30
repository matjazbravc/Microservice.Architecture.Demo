using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Exchange.Rates.Ecb.OpenApi.Filters
{
    /// <summary>
    /// Corresponding to Controller's API document description information
    /// </summary>
    public class SwaggerDocumentFilter : IDocumentFilter
    {
		private const string DOCS_URI = "https://exchangeratesapi.io/documentation/";

		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var tags = new List<OpenApiTag>
            {
                new()
                {
                    Name = "ExchangeRatesCoinCap",
                    Description = "Exchangerates API",
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