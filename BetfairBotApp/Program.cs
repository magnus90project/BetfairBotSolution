// BetfairBotApp/Program.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataHandling.Models;
using DataHandling.Repositories;
using DataHandling.Services;
using MachineLearning.Models;
using MachineLearning.Services;
using Serilog;
using BetfairAPI.Models;
using BetfairAPI.Services;

namespace BetfairBotApp
{
    class Program
    {
        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs\\BetfairBotApp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                // Användaruppgifter och API-nyckel
                var credentials = new BetfairCredentials
                {
                    Username = "fotboll53",
                    Password = "nana951dah!",
                    AppKey = "fdRyqNg9U2HvwJlk"
                };

                // Betfair-tjänst
                var betfairService = new BetfairService(credentials);
                await betfairService.LoginWithCertAsync();
                Log.Information("Authenticated successfully!");

                // Hämta marknadskatalog
                var marketCatalogue = await betfairService.FetchMarketCatalogueAsync("1"); // Exempel med filter för eventtyp
                Log.Information("Fetched market catalogue successfully!");

                // Ladda träningsdata
                var matchDataRepository = new MatchDataRepository("matchData.csv");
                var matchDataService = new MatchDataService(matchDataRepository);
                var matchDataList = matchDataService.GetMatchData();

                // Träna modellen
                var oddsPredictionService = new OddsPredictionService();
                oddsPredictionService.TrainModel(ConvertToMatchData(matchDataList));
                Log.Information("Model trained successfully!");

                // Hämta nya odds och göra prediktioner
                var newMatchData = new MatchData
                {
                    // Här skulle du fylla på med verklig data
                    OtherBookmakersOdds = new List<BookmakerOdds>
                    {
                        new BookmakerOdds { BookmakerName = "Bookmaker1", HomeWinOdds = 1.5f, DrawOdds = 3.5f, AwayWinOdds = 6.0f }
                    },
                    MatchDetails = new MatchDetails
                    {
                        MatchId = "123456",
                        Team1 = "Team A",
                        Team2 = "Team B",
                        MatchDate = DateTime.Now,
                        League = "Premier League",
                        Country = "UK",
                        Stadium = "Stadium Name",
                        InjuredPlayers = new List<PlayerStatus>(),
                        SuspendedPlayers = new List<PlayerStatus>(),
                        CurrentSeasonMatchNumber = 5,
                        TotalSeasonMatches = 38
                    },
                    HomeTeamInjurySuspensionImpact = 0.5f,
                    AwayTeamInjurySuspensionImpact = 0.3f,
                    BetfairHomeWinOdds = 1.6f,
                    BetfairDrawOdds = 3.4f,
                    BetfairAwayWinOdds = 5.5f
                };

                var prediction = oddsPredictionService.Predict(newMatchData);

                Log.Information($"Predicted Home Win Odds: {prediction.Prediction.PredictedHomeWinOdds}");
                Log.Information($"Predicted Draw Odds: {prediction.Prediction.PredictedDrawOdds}");
                Log.Information($"Predicted Away Win Odds: {prediction.Prediction.PredictedAwayWinOdds}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static IEnumerable<MachineLearning.Models.MatchData> ConvertToMatchData(IEnumerable<DataHandling.Models.MatchDetails> matchDetailsList)
        {
            var matchDataList = new List<MachineLearning.Models.MatchData>();

            foreach (var matchDetails in matchDetailsList)
            {
                var matchData = new MachineLearning.Models.MatchData
                {
                    OtherBookmakersOdds = new List<MachineLearning.Models.BookmakerOdds>
                    {
                        new MachineLearning.Models.BookmakerOdds
                        {
                            BookmakerName = "Bookmaker1",
                            HomeWinOdds = 1.5f,
                            DrawOdds = 3.5f,
                            AwayWinOdds = 6.0f
                        }
                    },
                    MatchDetails = matchDetails,
                    HomeTeamInjurySuspensionImpact = 0.5f,
                    AwayTeamInjurySuspensionImpact = 0.3f,
                    BetfairHomeWinOdds = 1.6f,
                    BetfairDrawOdds = 3.4f,
                    BetfairAwayWinOdds = 5.5f
                };

                matchDataList.Add(matchData);
            }

            return matchDataList;
        }
    }
}
