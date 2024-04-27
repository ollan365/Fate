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

    // �濡�� �̵� ��ư ������ ��
    public void MoveButtonEffect(GameObject screen, Vector3 direction)
    {
        StartCoroutine(OnMoveUI(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(null, 0, 1, 0, true, 0.2f, +0.25f));
    }

    // <summary> ���� ����
    // fadeObject�� fade ȿ���� ������ ��ü (null�� �ָ� ȭ�� ��ü)
    // start = 1, end = 0 �̸� ����� start = 0, end = 1�̸� ��ο���
    // fateTime�� �����(�Ǵ� ��ο���)�� �ɸ��� �ð�
    // blink�� true�̸� ��ο����ٰ� �����
    // waitingTime�� blink�� true�� �� ��ο��� �ִ� �ð�
    // changeFadeTime�� �ٽ� ����� �� �ɸ��� �ð��� �����ϰ� ������ ���� ����
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

        // ��ٷ� �ٽ� ��ο����ų� ������� �ϰ� ���� ��
        if (blink)
        {
            yield return new WaitForSeconds(waitingTime);
            StartCoroutine(OnFade(fadeObject, end, start, fadeTime + changeFadeTime, false, 0, 0));
        }

        // ������������ ����
        if (fadeObject == coverPanel && end == 0) fadeObject.gameObject.SetActive(false);
    }

    // <summary> ���� ����
    // ȭ�� �̵��� �� ����ϱ� ���� ���� �Ŷ� ������ ���� Ư���մϴ�...
    // screen�� ���� ��ġ�� �������� ������ �ǰ�,
    // ��������� ���� ��ġ(������)���� direction �������� distance ��ŭ �̵��� ���� �˴ϴ�
    // </summary>
    public IEnumerator OnMoveUI(GameObject screen, Vector3 direction, float distance, float time)
    {
        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();
        // ���� ��ġ (������)
        Vector3 originPosition = screenRectTransform.localPosition;
        // ��� ����
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

        // ���� ��ġ�� ����
        screen.GetComponent<RectTransform>().localPosition = originPosition;
    }
}
