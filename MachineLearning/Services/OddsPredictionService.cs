// Services/OddsPredictionService.cs
using MachineLearning.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearning.Services
{
    public class OddsPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public OddsPredictionService()
        {
            _mlContext = new MLContext();
        }

        public void TrainModel(IEnumerable<MatchData> trainingData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(MatchData.OtherBookmakersOdds), nameof(MatchData.MatchDetails), nameof(MatchData.HomeTeamInjurySuspensionImpact), nameof(MatchData.AwayTeamInjurySuspensionImpact))
                .Append(_mlContext.Transforms.Concatenate("Label", nameof(MatchData.BetfairHomeWinOdds), nameof(MatchData.BetfairDrawOdds), nameof(MatchData.BetfairAwayWinOdds)))
                .Append(_mlContext.Regression.Trainers.FastTree(new FastTreeRegressionTrainer.Options
                {
                    NumberOfLeaves = 20,
                    MinimumExampleCountPerLeaf = 10,
                    NumberOfTrees = 100,
                    LearningRate = 0.2
                }));

            _model = pipeline.Fit(dataView);
        }

        public MatchData Predict(MatchData matchData)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MatchData, MatchDataPrediction>(_model);
            var prediction = predictionEngine.Predict(matchData);

            matchData.Prediction = prediction;
            return matchData;
        }
    }
}
