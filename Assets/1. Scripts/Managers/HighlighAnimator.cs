using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class HighlightAnimator : MonoBehaviour
{
    [SerializeField] private Color  highlightColor = Color.red;
    [SerializeField] private float  pulseScale     = 1.1f;
    [SerializeField] private float  cycleDuration  = 1f;

    // Support both UI and Sprite renderers
    private Image          uiImage;
    private SpriteRenderer spriteRenderer;
    private Transform      targetTransform;

    private Color   originalColor;
    private Vector3 originalScale;

    private bool  isPulsing   = false;
    private float elapsedTime = 0f;

    private void Awake()
    {
        // Cache transform (works for both UI and sprites)
        targetTransform = transform;

        // Try to get an Image component...
        uiImage = GetComponent<Image>();
        if (uiImage != null)
        {
            originalColor = uiImage.color;
        }

        // ...and also try to get a SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;  // :contentReference[oaicite:0]{index=0}
        }

        // Cache the starting scale
        originalScale = targetTransform.localScale;  // :contentReference[oaicite:1]{index=1}
    }

    private void Update()
    {
        if (!isPulsing) return;

        // Advance time
        elapsedTime += Time.deltaTime;

        // t oscillates 0→1 in a sine wave over cycleDuration
        float t = (Mathf.Sin((elapsedTime / cycleDuration) * Mathf.PI * 2f) + 1f) * 0.5f;

        // Lerp scale
        targetTransform.localScale = Vector3.Lerp(
            originalScale,
            originalScale * pulseScale,
            t
        );

        // Lerp color on whichever component is present
        if (uiImage != null)
        {
            uiImage.color = Color.Lerp(originalColor, highlightColor, t);
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, t);  // :contentReference[oaicite:2]{index=2}
        }
    }

    /// <summary>
    /// Toggle pulsing on or off.
    /// </summary>
    public void ToggleHighlight(bool on)
    {
        isPulsing = on;

        if (!on)
        {
            // Reset state
            elapsedTime            = 0f;
            targetTransform.localScale = originalScale;

            if (uiImage != null)        uiImage.color        = originalColor;
            if (spriteRenderer != null) spriteRenderer.color = originalColor;
        }
    }
}
