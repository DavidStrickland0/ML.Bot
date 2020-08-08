namespace IntentRecognizer
{
    public interface IIntentRecognizerFacade
    {
        void Train(string path);
        IntentEnum Predict(string text);
    }
}