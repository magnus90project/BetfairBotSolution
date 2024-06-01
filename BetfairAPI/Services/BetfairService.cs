// Services/BetfairService.cs
using BetfairAPI.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BetfairAPI.Services
{
    public class BetfairService
    {
        private readonly BetfairCredentials _credentials;
        private string _sessionToken;
        private const string SuccessStatus = "SUCCESS";

        public BetfairService(BetfairCredentials credentials)
        {
            _credentials = credentials;
        }
        /*
        public async Task<string> LoginWithCertAsync()
        {
           // string certPath = "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\2024\\client-2048.crt";
            string certPath = "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\2024\\client-2048.pfx";


            string certPassword = "nana951dah";

            var certificate = new X509Certificate2(certPath, certPassword);
            var clientHandler = new HttpClientHandler();
            clientHandler.ClientCertificates.Add(certificate);
            var client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Add("X-Application", _credentials.AppKey);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", _credentials.Username),
                new KeyValuePair<string, string>("password", _credentials.Password)
            });

            var response = await client.PostAsync("https://identitysso-cert.betfair.se/api/certlogin", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + response.ReasonPhrase);
            }

            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = JObject.Parse(result);

            if ((string)jsonResult.loginStatus != SuccessStatus)
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + (string)jsonResult.loginStatus);
            }

            _sessionToken = jsonResult.sessionToken;
            return _sessionToken;
        
        }*/

        public async Task<string> LoginWithCertAsync()
        {
            string certPath = "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\2024\\client-2048.pfx";
            string certPassword = "nana951dah";

            var clientOptions = new RestClientOptions("https://identitysso-cert.betfair.se/api/certlogin")
            {
                ClientCertificates = new X509CertificateCollection
                {
                    new X509Certificate2(certPath, certPassword)
                }
            };

            var client = new RestClient(clientOptions);

            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddParameter("username", _credentials.Username);
            request.AddParameter("password", _credentials.Password);
            request.AddHeader("X-Application", _credentials.AppKey);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + response.ErrorMessage);
            }

            var result = response.Content;
            dynamic jsonResult = JObject.Parse(result);

            if ((string)jsonResult.loginStatus != SuccessStatus)
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + (string)jsonResult.loginStatus);
            }

            _sessionToken = jsonResult.sessionToken;
            return _sessionToken;
        }

        public async Task<List<MatchOdds>> FetchOddsAsync(string marketId)
        {
            if (string.IsNullOrEmpty(_sessionToken))
                throw new InvalidOperationException("You must authenticate first.");

            var client = new RestClient("https://api.betfair.com/exchange/betting/rest/v1.0/listMarketBook/");
            var request = new RestRequest(Method.Post.ToString());
            request.AddHeader("X-Application", _credentials.AppKey);
            request.AddHeader("X-Authentication", _sessionToken);
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                marketIds = new[] { marketId },
                priceProjection = new { priceData = new[] { "EX_BEST_OFFERS" } }
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = JArray.Parse(response.Content);
                var oddsList = new List<MatchOdds>();

                foreach (var market in content)
                {
                    var odds = new MatchOdds
                    {
                        MatchId = market["marketId"].ToString(),
                        MarketId = market["marketId"].ToString(),
                        Odds = market["runners"][0]["ex"]["availableToBack"][0]["price"].Value<decimal>(),
                        Timestamp = DateTime.UtcNow
                    };
                    oddsList.Add(odds);
                }
                return oddsList;
            }
            else
            {
                throw new Exception("Failed to fetch odds: " + response.ErrorMessage);
            }
        }

        public async Task<List<MarketCatalogue>> FetchMarketCatalogueAsync(string eventTypeId)
        {
            if (string.IsNullOrEmpty(_sessionToken))
                throw new InvalidOperationException("You must authenticate first.");

            var client = new RestClient("https://api.betfair.com/exchange/betting/rest/v1.0/listMarketCatalogue/");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("X-Application", _credentials.AppKey);
            request.AddHeader("X-Authentication", _sessionToken);
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                filter = new { eventTypeIds = new[] { eventTypeId } },
                maxResults = 100,
                marketProjection = new[] { "COMPETITION", "EVENT", "EVENT_TYPE", "MARKET_START_TIME", "RUNNER_DESCRIPTION" }
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = JArray.Parse(response.Content);
                var marketCatalogueList = new List<MarketCatalogue>();

                foreach (var market in content)
                {
                    var marketCatalogue = new MarketCatalogue
                    {
                        MarketId = market["marketId"].ToString(),
                        MarketName = market["marketName"].ToString(),
                        MarketStartTime = DateTime.Parse(market["marketStartTime"].ToString()),
                        TotalMatched = market["totalMatched"].Value<decimal>()
                    };
                    marketCatalogueList.Add(marketCatalogue);
                }
                return marketCatalogueList;
            }
            else
            {
                throw new Exception("Failed to fetch market catalogue: " + response.ErrorMessage);
            }
        }
    }
}
