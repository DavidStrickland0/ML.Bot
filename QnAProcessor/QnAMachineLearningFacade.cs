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
using Newtonsoft.Json;
using Tensorflow;

namespace QnAProcessor
{
    public class QnAMachineLearningFacade: IQnAMachineLearningFacade
    {
        private MLContext _mlContext;
        private ValueToKeyMappingEstimator _answerKeyEstimator;
        private EstimatorChain<ITransformer> _pipeline;
        private ITransformer _trainedModel;
        private PredictionEngine<MLEntry, AnswerPrediction> _predictionEngine;

        public QnAMachineLearningFacade(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public void Train(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader file = File.OpenText(path))
            {
                var qnaEnum = new List<MLEntry>();

                var qnaDictionary =
                    (Dictionary<string, List<string>>) serializer.Deserialize(file,
                        typeof(Dictionary<string, List<string>>));
                foreach (var answer in qnaDictionary.Keys)
                {
                    foreach (var question in qnaDictionary[answer])
                    {
                        qnaEnum.Add(new MLEntry()
                        {
                            Question = question,
                            Answer = answer
                        });
                    }
                }

                var shemaDef = SchemaDefinition.Create(typeof(MLEntry));
                shemaDef["PreviousIntents"].ColumnType = new VectorDataViewType(TextDataViewType.Instance);
                var data = _mlContext.Data.LoadFromEnumerable(qnaEnum);

                buildAndTrainModel(data, _pipeline);
            }
        }

        private void buildAndTrainModel(IDataView data, EstimatorChain<ITransformer> pipeline)
        {

            var trainingPipeline =
                _mlContext.Transforms.Conversion
                    .MapValueToKey(inputColumnName: nameof(MLEntry.Answer), outputColumnName: "Label")
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(MLEntry.Question),
                        outputColumnName: "QuestionFeaturized"))
                    .AppendCacheCheckpoint(_mlContext)
                    .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "QuestionFeaturized"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            _trainedModel = trainingPipeline.Fit(data);
            
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MLEntry, AnswerPrediction>(_trainedModel);

        }

        public string Predict(string text)
        {
            var entry = new MLEntry(){Question= text };

            var prediction = _predictionEngine.Predict(entry);
            return prediction.Answer;
        }
    }
}
