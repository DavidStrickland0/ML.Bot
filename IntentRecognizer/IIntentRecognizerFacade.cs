using System.Collections.Generic;

namespace IntentRecognizer
{
    public interface IIntentRecognizerFacade
    {
        void Train(string path);
        (IntentEnum, Dictionary<string, List<object>>) Predict(string text);
    }
}