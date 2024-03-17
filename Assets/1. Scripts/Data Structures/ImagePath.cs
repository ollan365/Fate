public class ImagePath
{
    public string ImageID { get; private set; }
    public string GirlPath { get; private set; }
    public string BoyPath { get; private set; }
    
    // initialize function
    public ImagePath(string id, string girlPath, string boyPath)
    {
        ImageID = id;
        GirlPath = girlPath;
        BoyPath = boyPath;
    }
}
