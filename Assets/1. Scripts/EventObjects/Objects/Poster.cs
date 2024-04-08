public class Poster : EventObject
{
    public new void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.Instance.IncrementVariable("PosterClick");
    }

}