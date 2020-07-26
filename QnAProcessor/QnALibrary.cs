using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace QnAProcessor
{
    public class QnALibrary : IQnALibrary
    {
        private IMachineLearningFacade _machineLearningFacade;
        private IDataView _dataview;
        public string Entries { get; set; }

        public QnALibrary(IMachineLearningFacade machineLearningFacade)
        {
            _machineLearningFacade = machineLearningFacade;
        }

        public void LoadFromTextFile(string path)
        {
            _machineLearningFacade.Train(path);

        }

        public string AnswerQuestion(string question)
        {
            return "Answer";
        }
    }
}
