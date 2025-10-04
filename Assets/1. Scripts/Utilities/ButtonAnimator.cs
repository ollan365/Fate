using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationType animationType = AnimationType.Pulse;
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 4f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Scale Animation")]
    [SerializeField] private float scaleAmount = 0.1f;
    
    [Header("Rotation Animation")]
    [SerializeField] private float rotationAmount = 5f;
    
    [Header("Movement Animation")]
    [SerializeField] private float moveAmount = 10f;
    [SerializeField] private Vector2 moveDirection = Vector2.up;
    
    [Header("Hover Effect")]
    [SerializeField] private bool enableHoverEffect = true;
    [SerializeField] private float hoverScaleAmount = 1.05f;
    
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine animationCoroutine;
    private Coroutine hoverCoroutine;
    private bool isHovering = false;
    
    public enum AnimationType
    {
        Rotate,
        // Move,
        Pulse,
        Wiggle,
        None,
    }
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
        
        // Add hover detection if needed
        if (enableHoverEffect)
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                #if UNITY_EDITOR || UNITY_STANDALONE
                EventTrigger trigger = GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = gameObject.AddComponent<EventTrigger>();
                
                // Add pointer enter and exit events
                AddEventTriggerListener(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
                AddEventTriggerListener(trigger, EventTriggerType.PointerExit, OnPointerExit);
                #endif
            }
        }
    }
    
    private void OnEnable()
    {
        // Start animation loop when enabled
        StopAllCoroutines();
        animationCoroutine = StartCoroutine(AnimationLoop());
    }
    
    private void OnDisable()
    {
        // Stop animations when disabled
        StopAllCoroutines();
        
        // Reset to original state
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
    }
    
    private IEnumerator AnimationLoop()
    {
        // Wait a bit before starting animations
        yield return new WaitForSecondsRealtime(Random.Range(0f, 1f));
        
        while (true)
        {
            // Wait random time between animations
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSecondsRealtime(waitTime);
            
            // Don't animate if hovering
            if (!isHovering)
            {
                // Play the selected animation type
                switch (animationType)
                {
                    case AnimationType.Rotate:
                        yield return StartCoroutine(RotateAnimation());
                        break;
                    // case AnimationType.Move:
                    //     yield return StartCoroutine(MoveAnimation());
                    //     break;
                    case AnimationType.Pulse:
                        yield return StartCoroutine(PulseAnimation());
                        break;
                    case AnimationType.Wiggle:
                        yield return StartCoroutine(WiggleAnimation());
                        break;
                    case AnimationType.None:
                    default:
                        break;
                }
            }
        }
    }
    
    private IEnumerator RotateAnimation()
    {
        Quaternion startRotation = rectTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAmount);
        Quaternion endRotation = Quaternion.Euler(0, 0, -rotationAmount);
        
        // Rotate one way
        float elapsed = 0f;
        while (elapsed < animationDuration / 3)
        {
            float t = elapsed / (animationDuration / 3);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Rotate the other way
        elapsed = 0f;
        while (elapsed < animationDuration / 3)
        {
            float t = elapsed / (animationDuration / 3);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localRotation = Quaternion.Lerp(targetRotation, endRotation, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Return to original rotation
        elapsed = 0f;
        while (elapsed < animationDuration / 3)
        {
            float t = elapsed / (animationDuration / 3);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localRotation = Quaternion.Lerp(endRotation, originalRotation, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Ensure we end at the original rotation
        rectTransform.localRotation = originalRotation;
    }
    
    private IEnumerator MoveAnimation()
    {
        Vector3 startPos = rectTransform.localPosition;
        Vector3 targetPos = startPos + (Vector3)(moveDirection.normalized * moveAmount);
        
        // Move to target
        float elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            float t = elapsed / (animationDuration / 2);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localPosition = Vector3.Lerp(startPos, targetPos, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Move back
        elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            float t = elapsed / (animationDuration / 2);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localPosition = Vector3.Lerp(targetPos, originalPosition, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Ensure we end at the original position
        rectTransform.localPosition = originalPosition;
    }
    
    private IEnumerator PulseAnimation()
    {
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * (1f + scaleAmount);
        
        // Scale up
        float elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            float t = elapsed / (animationDuration / 2);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Scale down
        elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            float t = elapsed / (animationDuration / 2);
            float curvedT = animationCurve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, curvedT);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Ensure we end at the original scale
        rectTransform.localScale = originalScale;
    }
    
    private IEnumerator WiggleAnimation()
    {
        Quaternion startRotation = rectTransform.localRotation;
        float wiggleTime = animationDuration / 5;
        
        // Create 5 quick rotations for wiggle effect
        for (int i = 0; i < 5; i++)
        {
            float direction = (i % 2 == 0) ? 1 : -1;
            Quaternion targetRotation = Quaternion.Euler(0, 0, direction * rotationAmount);
            
            float elapsed = 0f;
            while (elapsed < wiggleTime)
            {
                float t = elapsed / wiggleTime;
                rectTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
                
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            startRotation = targetRotation;
        }
        
        // Return to original rotation
        float returnElapsed = 0f;
        while (returnElapsed < wiggleTime)
        {
            float t = returnElapsed / wiggleTime;
            rectTransform.localRotation = Quaternion.Lerp(startRotation, originalRotation, t);
            
            returnElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // Ensure we end at the original rotation
        rectTransform.localRotation = originalRotation;
    }
    
    #if UNITY_EDITOR || UNITY_STANDALONE
    private void OnPointerEnter(BaseEventData eventData)
    {
        isHovering = true;
        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(HoverAnimation(true));
    }
    
    private void OnPointerExit(BaseEventData eventData)
    {
        isHovering = false;
        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(HoverAnimation(false));
    }
    
    private IEnumerator HoverAnimation(bool hovering)
    {
        Vector3 targetScale = hovering ? originalScale * hoverScaleAmount : originalScale;
        Vector3 currentScale = rectTransform.localScale;
        
        float elapsed = 0f;
        float hoverDuration = 0.2f;
        
        while (elapsed < hoverDuration)
        {
            float t = elapsed / hoverDuration;
            rectTransform.localScale = Vector3.Lerp(currentScale, targetScale, t);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        rectTransform.localScale = targetScale;
    }
    
    private void AddEventTriggerListener(EventTrigger trigger,
        EventTriggerType eventType,
        System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => callback(data));
        trigger.triggers.Add(entry);
    }
    #endif
}