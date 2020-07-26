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

namespace QnAProcessor
{
    public class QnAMachineLearningFacade: IMachineLearningFacade
    {
        private MLContext _mlContext;
        private ValueToKeyMappingEstimator _answerKeyEstimator;
        private EstimatorChain<ITransformer> _pipeline;
        private TransformerChain<KeyToValueMappingTransformer> _trainedModel;
        private PredictionEngine<QnAEntry, AnswerPrediction> _predictionEngine;

        public QnAMachineLearningFacade(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public void Train(string path)
        {
            var data = _mlContext.Data.LoadFromTextFile<QnAEntry>(path, new TextLoader.Options() { HasHeader = true });

            _pipeline = processData(path);

            buildAndTrainModel(data, _pipeline);

        }

        private void buildAndTrainModel(IDataView data, EstimatorChain<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "QuestionFeaturized"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            _trainedModel = trainingPipeline.Fit(data);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<QnAEntry, AnswerPrediction>(_trainedModel);

        }

        private EstimatorChain<ITransformer> processData(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new ApplicationException("path invalid");
            }


            _answerKeyEstimator = _mlContext.Transforms.Conversion.MapValueToKey(
                inputColumnName: nameof(QnAEntry.Answer),
                outputColumnName: "Label");

            return
                _answerKeyEstimator
                    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: nameof(QnAEntry.Question),
                        outputColumnName: "QuestionFeaturized"))
                    .AppendCacheCheckpoint(_mlContext);
        }

        public string Predict(string question)
        {
            var entry = new QnAEntry(){Question=question};

            var prediction = _predictionEngine.Predict(entry);
            return prediction.Answer;
        }
    }
}
