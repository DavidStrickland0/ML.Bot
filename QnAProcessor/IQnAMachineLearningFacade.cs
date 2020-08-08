using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace QnAProcessor
{
    public interface IQnAMachineLearningFacade
    {
        void Train(string path);
        string Predict(string text);
    }
}
