using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Fate.Utilities
{
    public class AlphaButton : MonoBehaviour
    {
        public float AlphaThreshold = 0.1f;

        void Start()
        {
            this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
        }

    }
}
