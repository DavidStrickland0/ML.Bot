using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Transforms.Text;
using Tensorflow;

namespace ContextualizedIntentRecognizer
{
    public class RecognizerMachineLearningFacade: IRecognizerMachineLearningFacade
    {
        private MLContext _mlContext;
        private ValueToKeyMappingEstimator _answerKeyEstimator;
        private EstimatorChain<ITransformer> _pipeline;
        private TransformerChain<KeyToValueMappingTransformer> _trainedModel;
        private PredictionEngine<MLEntry, IntentPrediction> _predictionEngine;

        public RecognizerMachineLearningFacade(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public void Train(string path)
        {
            var data = _mlContext.Data.LoadFromEnumerable<MLEntry>
            (
                new List<MLEntry>()
                {
                    new MLEntry()
                } 
            );

            buildAndTrainModel(data);
        }

        private void buildAndTrainModel(IDataView data)
        {
            _answerKeyEstimator = _mlContext.Transforms.Conversion.MapValueToKey(
                inputColumnName: nameof(MLEntry.Text),
                outputColumnName: "Label");

            var trainingPipeline =
                _answerKeyEstimator
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(MLEntry.Text),
                        outputColumnName: "TextFeaturized"))
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(MLEntry.Intent),
                        outputColumnName: "IntentFeaturized"))
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(MLEntry.PreviousIntents),
                        outputColumnName: "PreviousIntentFeaturized"))
                    .Append(_mlContext.Transforms.Concatenate("Featurized", "TextFeaturized", "IntentFeaturized", "PreviousIntentFeaturized"))
                    .AppendCacheCheckpoint(_mlContext)
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "QuestionFeaturized"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            _trainedModel = trainingPipeline.Fit(data);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MLEntry, IntentPrediction>(_trainedModel);

        }

        public string Predict(string text, string[] previousIntents)
        {
            var entry = new MLEntry(){Text =text, PreviousIntents = previousIntents };

            var prediction = _predictionEngine.Predict(entry);
            return prediction.Intent;
        }

    }
}
