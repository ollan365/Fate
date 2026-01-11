
namespace Fate.Data
{
    public class DialogueLine
    {
        public string SpeakerID { get; private set; }
        public string ScriptID { get; private set; }
        public string ImageID { get; private set; }
        public int ImageZoomLevel { get; private set; }
        public string BackgroundID { get; private set; }
        public string SoundID { get; private set; }
        public string Blur { get; private set; }
        public string Next { get; private set; }

        // initialize function
        public DialogueLine(string speakerID,
            string scriptID,
            string imageID,
            int imageZoomLevel,
            string backgroundID,
            string soundID,
            string blur,
            string next)
        {
            SpeakerID = speakerID;
            ScriptID = scriptID;
            ImageID = imageID;
            ImageZoomLevel = imageZoomLevel;
            BackgroundID = backgroundID;
            SoundID = soundID;
            Blur = blur;
            Next = next;
        }
    }
}
