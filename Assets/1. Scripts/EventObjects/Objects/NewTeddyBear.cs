using UnityEngine;

public class NewTeddyBear : EventObject
{
    private void Start()
    {
        if ((int)GameManager.Instance.GetVariable("ReplayCount") > 0)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}