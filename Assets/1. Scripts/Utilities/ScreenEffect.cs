using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect Instance { get; private set; }
    public Image coverPanel;
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

    // 방에서 이동 버튼 눌렀을 때
    public void MoveButtonEffect(GameObject screen, Vector3 direction)
    {
        StartCoroutine(OnMoveUI(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(null, 0, 1, 0, true, 0.2f, +0.25f));
    }

    // <summary> 변수 설명
    // fadeObject는 fade 효과를 적용할 물체 (null을 주면 화면 전체)
    // start = 1, end = 0 이면 밝아짐 start = 0, end = 1이면 어두워짐
    // fateTime은 밝아짐(또는 어두워짐)에 걸리는 시간
    // blink가 true이면 어두워졌다가 밝아짐
    // waitingTime은 blink가 true일 때 어두워져 있는 시간
    // changeFadeTime은 다시 밝아질 때 걸리는 시간을 조정하고 싶으면 쓰는 변수
    // </summary>
    public IEnumerator OnFade(Image fadeObject, float start, float end, float fadeTime, bool blink, float waitingTime, float changeFadeTime)
    {
        if (fadeObject == null) fadeObject = coverPanel;

        if (!fadeObject.gameObject.activeSelf) fadeObject.gameObject.SetActive(true);
        Color newColor = fadeObject.color;
        newColor.a = start;
        fadeObject.color = newColor;

        float current = 0, percent = 0;

        while (percent < 1 && fadeTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            newColor.a = Mathf.Lerp(start, end, percent);
            fadeObject.color = newColor;

            yield return null;
        }
        newColor.a = end;
        fadeObject.color = newColor;

        // 곧바로 다시 어두워지거나 밝아지게 하고 싶을 때
        if (blink)
        {
            yield return new WaitForSeconds(waitingTime);
            StartCoroutine(OnFade(fadeObject, end, start, fadeTime + changeFadeTime, false, 0, 0));
        }

        // 투명해졌으면 끈다
        if (fadeObject == coverPanel && end == 0) fadeObject.gameObject.SetActive(false);
    }

    // <summary> 변수 설명
    // 화면 이동할 때 사용하기 위해 만든 거라 동작이 조금 특이합니다...
    // screen의 현재 위치가 목적지로 설정이 되고,
    // 출발지점은 현재 위치(목적지)에서 direction 방향으로 distance 만큼 이동한 곳이 됩니다
    // </summary>
    public IEnumerator OnMoveUI(GameObject screen, Vector3 direction, float distance, float time)
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

        // 원래 위치로 설정
        screen.GetComponent<RectTransform>().localPosition = originPosition;
    }
}
