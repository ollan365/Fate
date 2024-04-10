using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect Instance { get; private set; }
    [SerializeField] private Image coverPanel;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 방에서 이동 버튼 눌렀을 때 (영상이랑 비슷한 값으로 조정)
    public void MoveButtonEffect(GameObject screen, Vector3 direction)
    {
        StartCoroutine(OnMove(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(0, 1, 0.2f, true, 0.2f, +0.05f));
    }

    // <summary> 변수 설명
    // start = 1, end = 0 이면 밝아짐 start = 0, end = 1이면 어두워짐
    // fateTime은 밝아짐(또는 어두워짐)에 걸리는 시간
    // blink가 true이면 어두워졌다가 밝아짐
    // waitingTime은 blink가 true일 때 어두워져 있는 시간
    // changeFadeTime은 다시 밝아질 때 걸리는 시간을 조정하고 싶으면 쓰는 변수
    // </summary>
    public IEnumerator OnFade(float start, float end, float fadeTime, bool blink, float waitingTime, float changeFadeTime)
    {
        if (!coverPanel.gameObject.activeSelf) coverPanel.gameObject.SetActive(true);
        coverPanel.color = new(0, 0, 0, start);

        float current = 0, percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            coverPanel.color = new(0, 0, 0, Mathf.Lerp(start, end, percent));

            yield return null;
        }

        // 곧바로 다시 어두워지거나 밝아지게 하고 싶을 때
        if (blink)
        {
            yield return new WaitForSeconds(waitingTime);
            StartCoroutine(OnFade(end, start, fadeTime + changeFadeTime, false, 0, 0));
        }

        // 투명해졌으면 끈다
        if (end == 0) coverPanel.gameObject.SetActive(false);
    }

    public IEnumerator OnMove(GameObject screen, Vector3 direction, float distance, float time)
    {
        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();
        // 원래 위치 (목적지)
        Vector3 originPosition = screenRectTransform.localPosition;
        // 출발 지점
        Vector3 startPosition = screenRectTransform.localPosition + direction * distance;
        screen.GetComponent<RectTransform>().localPosition = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsedTime / time);

            screen.transform.localPosition = Vector3.Lerp(startPosition, originPosition, percent);

            yield return null;
        }

        // 원래 위치로 재설정
        screen.GetComponent<RectTransform>().localPosition = originPosition;
    }
}
