public class Phone : EventObject
{
    private new void OnMouseDown()
    {
        base.OnMouseDown();
        // GameManager.Instance.IncrementPhoneCalled();
    }
}