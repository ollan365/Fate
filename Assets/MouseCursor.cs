using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D busyCursor;
    private readonly Vector2 hotSpot = new Vector2(-1, 1);
    
    void Awake()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }
    
    public void SetBusyMode() 
    {
        Cursor.SetCursor(busyCursor, hotSpot, CursorMode.Auto);
    }
    
    public void SetDefaultMode() 
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }
}
