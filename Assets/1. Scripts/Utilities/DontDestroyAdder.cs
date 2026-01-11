using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fate.Utilities
{
    public class DontDestroyAdder : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
