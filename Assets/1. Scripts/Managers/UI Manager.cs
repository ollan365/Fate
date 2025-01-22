using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Game Objects")]
    public GameObject NormalVignette;
    
    public Dictionary<string, GameObject> UIGameObjects = new Dictionary<string, GameObject>();
    
    public static UIManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
        AddUIGameObjects();
        SetAllUI(false);
    }
    
    private void AddUIGameObjects()
    {
        UIGameObjects.Add("NormalVignette", NormalVignette);
    }
    
    public void SetAllUI(bool isActive)
    {
        foreach (var ui in UIGameObjects)
            SetUI(ui.Key, isActive);
    }

    public void SetUI(string uiName, bool isActive)
    {
        UIGameObjects[uiName].SetActive(isActive);
    }
    
}
