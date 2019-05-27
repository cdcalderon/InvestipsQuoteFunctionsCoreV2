using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using YahooFinanceApi;

namespace CreateStockQuote
{

    public static class CreateStockQuote
    {
        [FunctionName("CreateStockQuote")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Quote quote = new Quote();
            quote.Symbol = "AAPL";

            var uri = new Uri("https://investips2019.documents.azure.com:443/");
            var key = "0AZIIWw8IENGZdFXZSzN4BzZvmvhom5xgVLzsIvbhZBItebHsz3y8XL4qdMCM4NSSwmFCMd4J2mCRD4FbHCZcw==";
            var quotesLink = UriFactory.CreateDocumentCollectionUri("investips", "quotes");
            var option = new FeedOptions { EnableCrossPartitionQuery = true };

            var client = new DocumentClient(uri, key);

            await client.CreateDocumentAsync(quotesLink, quote);

            log.LogInformation("Saved Quote");

            var justSavedQuote = client.CreateDocumentQuery<Quote>(quotesLink, option)
                                       .OrderBy(q => q.Symbol).ToList();

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var quotes = await Yahoo.GetHistoricalAsync("AAPL", new DateTime(2018, 12, 1), DateTime.Now, Period.Daily);

            var resultState = TicTacTec.TA.Library.Core.MovingAverage(
                    0,
                    closePrices.Length - 1,
                    closePrices, period,
                    Core.MAType.Ema,
                    out var outBegIndex,
                    out var outNbElement,
                    outMovingAverages);





            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");


        }
    }

    public class Quote
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Symbol { get; set; }
    }

    public class QuoteRead
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string Symbol { get; set; }
    }
}
