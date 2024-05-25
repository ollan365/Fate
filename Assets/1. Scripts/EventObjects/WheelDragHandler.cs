using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDragHandler : MonoBehaviour
{
    [SerializeField] private Wheel wheel;
    [SerializeField] private float rotationThreshold = 50f;
    
    private Vector3 lastMousePosition;

    void Update()
    {
        bool isRotating = wheel.isRotating;
        if (!isRotating)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) >= rotationThreshold) // Check if vertical movement is greater than horizontal
                {
                    float angle = 36 * (delta.y > 0 ? 1 : -1); // Determine the direction based on drag
                    StartCoroutine(wheel.RotateWheel(angle));
                }
                lastMousePosition = Input.mousePosition;
            }
        }
    }

}
