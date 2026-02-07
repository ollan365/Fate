using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float pressedScale = 1.15f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private Color pressedColor = new Color(1f, 0.7f, 0.85f, 1f); // Pink tint

    private Vector3 originalScale;
    private Color originalColor;
    private Graphic targetGraphic;
    private Coroutine currentAnimation;

    private void Awake()
    {
        originalScale = transform.localScale;

        // Find text component for color change
        targetGraphic = GetComponentInChildren<TextMeshProUGUI>();
        if (targetGraphic == null)
            targetGraphic = GetComponentInChildren<Graphic>();

        if (targetGraphic != null)
            originalColor = targetGraphic.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (HapticManager.Instance) HapticManager.Instance.PlayImpact();
        AnimateTo(originalScale * pressedScale, pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(originalScale, originalColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateTo(originalScale, originalColor);
    }

    private void AnimateTo(Vector3 targetScale, Color targetColor)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(PressAnimation(targetScale, targetColor));
    }

    private IEnumerator PressAnimation(Vector3 targetScale, Color targetColor)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = targetGraphic != null ? targetGraphic.color : originalColor;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            // Ease out for snappy feel
            t = 1f - (1f - t) * (1f - t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            if (targetGraphic != null)
                targetGraphic.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        transform.localScale = targetScale;
        if (targetGraphic != null)
            targetGraphic.color = targetColor;
        currentAnimation = null;
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
        if (targetGraphic != null)
            targetGraphic.color = originalColor;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }
}
