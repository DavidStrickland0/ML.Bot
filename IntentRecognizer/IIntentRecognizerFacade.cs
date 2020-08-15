using System.Collections.Generic;

namespace IntentRecognizer
{
    public interface IIntentRecognizerFacade
    {
        void Train(string path);
        (string, Dictionary<string, IList<object>>) Predict(string text);
    }
}