using System;
using System.Collections;
using UnityEngine;
using Fate.Managers;


namespace Fate.Events
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private GameObject[] numbers;
        [SerializeField] private float rotationTime = 0.3f;

        [SerializeField] private TinCaseLock tinCaseLock;
    
        public int currentNumber;
        public bool isRotating;

        public IEnumerator RotateWheel(float angle)
        {
            isRotating = true;

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(angle, 0, 0);

            float elapsed = 0f;
            while (elapsed < rotationTime)
            {
                elapsed += Time.deltaTime;
                float fraction = elapsed / rotationTime;
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, fraction);
                yield return null;
            }

            transform.rotation = endRotation;
            currentNumber += angle > 0 ? 1 : -1;
            currentNumber = (currentNumber + 10) % 10;
        
            //tinCaseLock.CheckAnswer();

            isRotating = false;
        }
    
        private void InitializeNumbers()
        {
            currentNumber = 0;

            var hypotenuse = radius;

            for (int i = 0; i < 10; i++)
            {
                float angle = Mathf.Deg2Rad * (36 * i);  // 360 / 10 = 36
                float sinAngle = (float) Math.Round(Mathf.Sin(angle),2);
                float cosAngle = (float) Math.Round(Mathf.Cos(angle), 2);
        
                Vector3 numberPosition = new Vector3(0, -sinAngle * hypotenuse, -cosAngle * hypotenuse);
                numbers[i].transform.localPosition = numberPosition;
        
                numbers[i].transform.rotation = Quaternion.Euler(-36 * i, 0, 0);
        
            }
        }
    }
}
