using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.ML;
using NUnit.Framework;

namespace QnAProcessor.Tests
{
    public class QnALibraryTests

    {
        private QnAMachineLearningFacade _facade;

        [SetUp]
        public void Setup()
        {

            var resourceName = "QnA.tsv";

            _facade = new QnAMachineLearningFacade(new MLContext());
            _facade.Train(System.IO.Path.Combine(resourceName));
        }

        [TestCase("Do you pee?", "I don't have a body.")]
        [TestCase("Can you pee?", "I don't have a body.")]
        public void LoadQnALibrary(string question, string answer)
        {

            var response = _facade.Predict(question);
            Assert.AreEqual(answer,response);
        }
    }
}