public static class Constants
{
    [System.Serializable]
    public enum File { Localization, ChoiceEvent, Prologue }
    public static string ToString (this File file)
    {
        switch (file)
        {
            case File.Localization: return "Localization";
            case File.ChoiceEvent: return "ChoiceEvent";
            case File.Prologue: return "Prologue";

            default: return null;
        }
    }
}
