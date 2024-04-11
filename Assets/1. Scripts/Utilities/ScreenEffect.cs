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

    // �濡�� �̵� ��ư ������ �� (�����̶� ����� ������ ����)
    public void MoveButtonEffect(GameObject screen, Vector3 direction)
    {
        StartCoroutine(OnMoveUI(screen, direction, 100, 0.5f));
        StartCoroutine(OnFade(0, 1, 0, true, 0.2f, +0.25f));
    }

    // <summary> ���� ����
    // start = 1, end = 0 �̸� ����� start = 0, end = 1�̸� ��ο���
    // fateTime�� �����(�Ǵ� ��ο���)�� �ɸ��� �ð�
    // blink�� true�̸� ��ο����ٰ� �����
    // waitingTime�� blink�� true�� �� ��ο��� �ִ� �ð�
    // changeFadeTime�� �ٽ� ����� �� �ɸ��� �ð��� �����ϰ� ������ ���� ����
    // </summary>
    public IEnumerator OnFade(float start, float end, float fadeTime, bool blink, float waitingTime, float changeFadeTime)
    {
        if (!coverPanel.gameObject.activeSelf) coverPanel.gameObject.SetActive(true);
        coverPanel.color = new(0, 0, 0, start);

        float current = 0, percent = 0;

        while (percent < 1 && fadeTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            coverPanel.color = new(0, 0, 0, Mathf.Lerp(start, end, percent));

            yield return null;
        }
        coverPanel.color = new(0, 0, 0, end);

        // ��ٷ� �ٽ� ��ο����ų� ������� �ϰ� ���� ��
        if (blink)
        {
            yield return new WaitForSeconds(waitingTime);
            StartCoroutine(OnFade(end, start, fadeTime + changeFadeTime, false, 0, 0));
        }

        // ������������ ����
        if (end == 0) coverPanel.gameObject.SetActive(false);
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

        // ���� ��ġ�� �缳��
        screen.GetComponent<RectTransform>().localPosition = originPosition;
    }
}
