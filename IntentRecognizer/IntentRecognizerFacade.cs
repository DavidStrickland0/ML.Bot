using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.ML.Data;
using Newtonsoft.Json;

namespace IntentRecognizer
{
    public class IntentRecognizerFacade: IIntentRecognizerFacade
    {
        private EstimatorChain<ITransformer> _pipeline;
        private MLContext _mlContext;
        private ITransformer _trainedModel;
        private PredictionEngine<MLEntry,IntentPrediction> _predictionEngine;

        public IntentRecognizerFacade(MLContext mlContext)
        {
            _mlContext = mlContext;
        }
        public void Train(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader file = File.OpenText(path))
            {
                var intentEnum = new List<MLEntry>();

                var intentDictionary =
                    (Dictionary<string, List<string>>)serializer.Deserialize(file,
                        typeof(Dictionary<string, List<string>>));
                foreach (var intent in intentDictionary.Keys)
                {
                    foreach (var statement in intentDictionary[intent])
                    {
                        intentEnum.Add(new MLEntry()
                        {
                            Statement = statement,
                            Intent = intent
                        });
                    }
                }

                var shemaDef = SchemaDefinition.Create(typeof(MLEntry));
                var data = _mlContext.Data.LoadFromEnumerable(intentEnum);

                buildAndTrainModel(data, _pipeline);
            }
        }

        private void buildAndTrainModel(IDataView data, EstimatorChain<ITransformer> pipeline)
        {

            var trainingPipeline =
                _mlContext.Transforms.Conversion
                    .MapValueToKey(inputColumnName: nameof(MLEntry.Intent), outputColumnName: "Label")
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(MLEntry.Statement),
                        outputColumnName: "StatementFeaturized"))
                    .AppendCacheCheckpoint(_mlContext)
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "StatementFeaturized"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            _trainedModel = trainingPipeline.Fit(data);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MLEntry, IntentPrediction>(_trainedModel);

        }

        public (IntentEnum, Dictionary<string, List<object>>) Predict(string text)
        {
            var entry = new MLEntry() { Statement = text };

            var prediction = _predictionEngine.Predict(entry);
            Enum.TryParse<IntentEnum>(prediction.Intent,out var result);
            return (result,null);
        }
    }
}
