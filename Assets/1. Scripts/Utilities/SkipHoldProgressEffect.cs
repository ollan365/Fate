using TMPro;
using UnityEngine;

public class SkipHoldProgressEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Color baseColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color highlightColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] [Range(0f, 0.5f)] private float featherWidth = 0.1f; // normalized width of the soft edge
    [Header("Completion Feedback")] [SerializeField] private bool playCompletionPulse = true;
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseDuration = 0.12f; // up and down each ~duration

    private bool initialized;
    private bool completedNotified;
    private RectTransform rectTransform;
    private Vector3 initialScale;
    private Vector2 initialAnchoredPos;
    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (!targetText)
            targetText = GetComponent<TextMeshProUGUI>();
        rectTransform = targetText ? targetText.rectTransform : GetComponent<RectTransform>();
        if (rectTransform)
        {
            initialScale = rectTransform.localScale;
            initialAnchoredPos = rectTransform.anchoredPosition;
        }
    }

    private void EnsureInitialized()
    {
        if (initialized)
            return;

        if (!targetText)
            targetText = GetComponent<TextMeshProUGUI>();

        initialized = targetText != null;
    }

    public void UpdateProgress(float progress)
    {
        EnsureInitialized();
        if (!initialized)
            return;

        progress = Mathf.Clamp01(progress);

        targetText.ForceMeshUpdate();
        var textInfo = targetText.textInfo;
        int characterCount = textInfo.characterCount;
        if (characterCount == 0)
            return;

        // Hard cases: 0% or 100% progress → no gradient at all
        if (progress <= 0f)
        {
            completedNotified = false;
            ResetTransformVisual();
            for (int i = 0; i < characterCount; i++)
            {
                var ci = textInfo.characterInfo[i];
                if (!ci.isVisible) continue;
                int mi = ci.materialReferenceIndex;
                int vi = ci.vertexIndex;
                var colors = textInfo.meshInfo[mi].colors32;
                Color32 c = (Color32)baseColor;
                colors[vi + 0] = c;
                colors[vi + 1] = c;
                colors[vi + 2] = c;
                colors[vi + 3] = c;
            }
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.colors32 = meshInfo.colors32;
                targetText.UpdateGeometry(meshInfo.mesh, i);
            }
            return;
        }

        if (progress >= 1f)
        {
            if (!completedNotified)
            {
                completedNotified = true;
                if (playCompletionPulse)
                    StartCompletionPulse();
            }
            for (int i = 0; i < characterCount; i++)
            {
                var ci = textInfo.characterInfo[i];
                if (!ci.isVisible) continue;
                int mi = ci.materialReferenceIndex;
                int vi = ci.vertexIndex;
                var colors = textInfo.meshInfo[mi].colors32;
                Color32 c = (Color32)highlightColor;
                colors[vi + 0] = c;
                colors[vi + 1] = c;
                colors[vi + 2] = c;
                colors[vi + 3] = c;
            }
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.colors32 = meshInfo.colors32;
                targetText.UpdateGeometry(meshInfo.mesh, i);
            }
            return;
        }

        // mid-range progress: allow next completion pulse when it reaches 1 again
        completedNotified = false;

        // Determine horizontal bounds across all visible characters
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int meshIndexB = charInfo.materialReferenceIndex;
            int vertexIndexB = charInfo.vertexIndex;
            var verticesB = textInfo.meshInfo[meshIndexB].vertices;

            // four vertices of the quad
            for (int v = 0; v < 4; v++)
            {
                float x = verticesB[vertexIndexB + v].x;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
            }
        }

        if (!float.IsFinite(minX) || !float.IsFinite(maxX) || Mathf.Approximately(maxX - minX, 0f))
            return;

        // Apply per-vertex gradient based on x-position
        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            var vertices = textInfo.meshInfo[meshIndex].vertices;
            var colors = textInfo.meshInfo[meshIndex].colors32;

            for (int v = 0; v < 4; v++)
            {
                float x = vertices[vertexIndex + v].x;
                float xNorm = Mathf.InverseLerp(minX, maxX, x);

                // Soft step around the progress position using featherWidth
                float t = Mathf.InverseLerp(progress + featherWidth, progress - featherWidth, xNorm);
                Color blended = Color.Lerp(baseColor, highlightColor, t);
                colors[vertexIndex + v] = (Color32)blended;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.colors32 = meshInfo.colors32;
            targetText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void ResetEffect()
    {
        UpdateProgress(0f);
    }

    private void StartCompletionPulse()
    {
        if (!rectTransform)
            return;
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(PulseRoutine());
    }

    private System.Collections.IEnumerator PulseRoutine()
    {
        float half = Mathf.Max(0.01f, pulseDuration);
        float t = 0f;
        // scale up
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            float s = Mathf.Lerp(initialScale.x, pulseScale, k);
            rectTransform.localScale = new Vector3(s, s, initialScale.z);
            yield return null;
        }
        t = 0f;
        // scale down
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            float s = Mathf.Lerp(pulseScale, initialScale.x, k);
            rectTransform.localScale = new Vector3(s, s, initialScale.z);
            yield return null;
        }
        rectTransform.localScale = initialScale;
        feedbackRoutine = null;
    }

    private void ResetTransformVisual()
    {
        if (!rectTransform)
            return;
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }
        rectTransform.localScale = initialScale;
        rectTransform.anchoredPosition = initialAnchoredPos;
    }
}


