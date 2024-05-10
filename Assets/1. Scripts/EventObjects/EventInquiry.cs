using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInquiry : EventObject
{
    [SerializeField]
    protected string objectName;

    public new void OnMouseDown()
    {
        RoomManager.Instance.setCurrentInquiryObjectName(objectName);
        base.OnMouseDown();
    }

    public string GetObjectName()
    {
        return objectName;
    }
}
