using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(RectTransform))]
public class HighlightAnimator : MonoBehaviour
{
    [SerializeField] private Color  highlightColor = Color.red;
    [SerializeField] private float  pulseScale     = 1.1f;
    [SerializeField] private float  cycleDuration  = 1f;

    private Image         imageComp;
    private RectTransform rectTransform;
    private Color         originalColor;
    private Vector3       originalScale;

    private bool  isPulsing   = false;
    private float elapsedTime = 0f;

    private void Awake()
    {
        imageComp     = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        originalColor = imageComp.color;
        originalScale = rectTransform.localScale;
    }

    private void Update()
    {
        if (!isPulsing)
            return;

        elapsedTime += Time.deltaTime;

        float t = (Mathf.Sin((elapsedTime / cycleDuration) * Mathf.PI * 2f) + 1f) * 0.5f;

        rectTransform.localScale = Vector3.Lerp(
            originalScale,
            originalScale * pulseScale,
            t
        );

        imageComp.color = Color.Lerp(
            originalColor,
            highlightColor,
            t
        );
    }

    public void ToggleHighlight(bool on)
    {
        isPulsing = on;

        if (!on)
        {
            elapsedTime               = 0f;
            rectTransform.localScale  = originalScale;
            imageComp.color           = originalColor;
        }
    }
}
