using UnityEngine;

public class WheelDragHandler : MonoBehaviour
{
    [SerializeField] private Wheel wheel;
    [SerializeField] private float rotationThreshold = 50f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxDistance = 100f;
    
    private Vector3 lastMousePosition;
    private float accumulatedDeltaY = 0;
    private bool isDragging = false;

    void Update()
    {
        if (wheel.isRotating) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            // 비밀번호 무한 입력 시도 방지
            RoomManager.Instance.ProhibitInput();

            if (!IsMouseOverWheel()) return;
            
            lastMousePosition = Input.mousePosition;
            accumulatedDeltaY = 0;
            isDragging = true;
            SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_TincaseScroll, true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SoundPlayer.Instance.UISoundPlay_LOOP(Constants.Sound_TincaseScroll, false);
            isDragging = false;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            if (!IsMouseOverWheel()) return;
            
            Vector3 delta = Input.mousePosition - lastMousePosition;
            accumulatedDeltaY += delta.y;

            if (Mathf.Abs(accumulatedDeltaY) >= rotationThreshold)
            {
                float angle = 36 * (accumulatedDeltaY > 0 ? 1 : -1);
                StartCoroutine(wheel.RotateWheel(angle));
                accumulatedDeltaY = 0;
            }

            lastMousePosition = Input.mousePosition;
        }
    }

    private bool IsMouseOverWheel()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, maxDistance)) return hit.collider.gameObject == gameObject;
        
        return false;
    }
}
