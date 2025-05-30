using HtmlAgilityPack;
using System.Text.Json;
using System.Text;
namespace MyPortfolio.Services
{
    public interface IStockPriceService
    {
        Task<Dictionary<string, double>> GetPriceAsync();
    }

    public class StockPriceService : IStockPriceService
    {
        private readonly HttpClient _httpClient;

        public StockPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, double>> GetPriceAsync()
        {
            Dictionary<string, double> closeHash = new Dictionary<string, double>();

            var url = "https://groww.in/v1/api/stocks_data/v1/all_stocks";

            var headers = new Dictionary<string, string>
            {
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0" }
            };

            //using (HttpClient client = new HttpClient())
            //{
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }

                for (int i = 0; i < 7; i++)
                {
                    var postData = new
                    {
                        listFilters = new
                        {
                            INDUSTRY = new string[] { },
                            INDEX = new string[] { }
                        },
                        objFilters = new
                        {
                            CLOSE_PRICE = new { max = 100000, min = 0 },
                            MARKET_CAP = new { min = 0, max = 3000000000000000 }
                        },
                        page = i,
                        size = "1000",
                        sortBy = "COMPANY_NAME",
                        sortType = "ASC"
                    };

                    var content = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    var records = doc.RootElement.GetProperty("records");

                    foreach (var record in records.EnumerateArray())
                    {
                        double close = 0;
                        string nseCode = null;

                        //if (record.TryGetProperty("ltp", out JsonElement ltpElement) && ltpElement.ValueKind == JsonValueKind.Number)
                        //{
                        //    close = ltpElement.GetDouble();
                        //}

                        if (record.TryGetProperty("livePriceDto", out JsonElement livePriceDto) &&
                            livePriceDto.ValueKind == JsonValueKind.Object && livePriceDto.TryGetProperty("ltp", out JsonElement ltpElement) && ltpElement.ValueKind == JsonValueKind.Number)
                        {
                            close = ltpElement.GetDouble();
                        }

                        if (record.TryGetProperty("nseScriptCode", out JsonElement nseElement) && nseElement.ValueKind != JsonValueKind.Null)
                        {
                            nseCode = nseElement.GetString();
                        }
                        //else if (record.TryGetProperty("bseScriptCode", out JsonElement bseElement) && bseElement.ValueKind == JsonValueKind.Number)
                        //{
                        //    nseCode = bseElement
                        //}

                        if (nseCode != null)
                        {
                            closeHash[nseCode] = close;
                        }
                    }
                }
                return closeHash;
            //}
        }
    }
}
