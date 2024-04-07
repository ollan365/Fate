using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAdder : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
