using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject followCanvas;
    [SerializeField] private GameObject miniGameCanvas;

    [Header("UI")]
    [SerializeField] private Slider gaugeSlider;
    [SerializeField] private Animator[] heartAnimator;
    [SerializeField] private Button memoButton;

    [Header("Character")]
    [SerializeField] private Image accidy;
    [SerializeField] private GameObject fateBack, fateSit;
    [SerializeField] private Sprite accidyGirlFront, accidyGirlBack, accidyBoyFront, accidyBoyBack;
    private Sprite accidyFrontSprite, accidyBackSprite;

    // ��ü Ŭ�� Ƚ��
    private float clickCount;
    public float ClickCount
    {
        get => clickCount;
        set
        {
            clickCount = value;

            if (clickCount % 5 == 0) StartCoroutine(StartMiniGame());
        }
    }

    // ���º���
    private int heartCount = 3; // ��� ����
    private bool isGameOver; // ���� ���� �Ǿ��°�
    private bool accidyBack; // �쿬�� �ڸ� ���� �ִ°�
    private bool fateMove; // �ʿ��� �̵��� �ߴ°�

    private void Start()
    {
        // �쿬�� ������ ���� �ٸ� �̹���
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0)
        {
            accidyFrontSprite = accidyGirlFront;
            accidyBackSprite = accidyGirlBack;
        }
        else
        {
            accidyFrontSprite = accidyBoyFront;
            accidyBackSprite = accidyBoyBack;
        }
        accidy.sprite = accidyFrontSprite;
        accidy.SetNativeSize();
    }
    private IEnumerator StartMiniGame()
    {
        // ������ �ʱ�ȭ
        isGameOver = false;
        accidyBack = false;
        fateMove = false;
        gaugeSlider.value = 0;

        // �޸� ��ư ���ֱ�
        MemoManager.Instance.HideMemoButton(true);

        // ���̵� �ƿ��� ���� �ϸ� ���� ĵ������ ���� �̴� ���� ĵ������ �Ҵ�
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        yield return new WaitForSeconds(0.4f); // ���̵� �� �ƿ� ��

        // �쿬�� ������ ����
        StartCoroutine(AccidyLogic());

        // ������ �޸����� ��ư Ȱ��ȭ
        memoButton.gameObject.SetActive(true);

        // �ʿ��� �������� �쿬�� �ڸ� ���ƺ� ���°� ��ø�Ǹ� ��Ʈ �ϳ� ����
        while (!isGameOver)
        {
            if(fateMove && accidyBack)
            {
                heartCount--;

                if (heartCount <= 0) isGameOver = true;
                else // ü���� 1 ��� �ٽ� �������� ���ư���
                {
                    fateMove = false;
                    StartCoroutine(FateMove(false));

                    // ��Ʈ�� ������ �ִϸ��̼� ���
                    heartAnimator[heartCount].SetTrigger("Break");

                    // ȭ���� �Ӿ����� �ִϸ��̼�
                    ScreenEffect.Instance.coverPanel.color = Color.red;
                    StartCoroutine(ScreenEffect.Instance.OnFade(null, 0.5f, 0, 0.2f, false, 0, 0));
                    yield return new WaitForSeconds(0.5f);

                    heartAnimator[heartCount].gameObject.SetActive(false);
                    ScreenEffect.Instance.coverPanel.color = Color.black;
                }
            }
            else if (!fateMove) // �ʿ��� �������� ������ õõ�� ������ ����
            {
                gaugeSlider.value -= 0.0001f;
            }

            yield return null;
        }

        // ���� ���� (����� ������ �������� ���ư�)
        MemoManager.Instance.HideMemoButton(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        miniGameCanvas.SetActive(false);
        followCanvas.SetActive(true);
        yield return new WaitForSeconds(0.4f); // ���̵� �� �ƿ� ��
    }

    public void MemoButtonOnClick()
    {
        if (!fateMove) // �ʿ��� �����ִ� ���¿��ٸ�
        {
            StartCoroutine(FateMove(true));
        }
        else
        {
            gaugeSlider.value += 0.015f;

            if(gaugeSlider.value == 1) isGameOver = true;
        }
    }
    public void WallButtonOnClick()
    {
        StartCoroutine(FateMove(false));
    }
    private IEnumerator FateMove(bool goToMemo)
    {
        memoButton.gameObject.SetActive(false); // �ִϸ��̼� ���� ������ ��ư ��Ȱ��ȭ

        if (goToMemo)
        {
            // �ʿ��� ��ġ�� �ٲ�� �ִϸ��̼�
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 1, 0, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit.GetComponent<Image>(), 0, 1, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);

            // �ʿ��� ���¸� �̵��� ���·� ����
            fateMove = true;
        }
        else
        {
            // �ʿ��� ���¸� �̵����� ���� ���·� ����
            fateMove = false;

            // �ʿ��� ��ġ�� �ٲ�� �ִϸ��̼�
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit.GetComponent<Image>(), 1, 0, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 0, 1, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);
        }
        memoButton.gameObject.SetActive(true);
    }
    private IEnumerator AccidyLogic()
    {
        // ���̵�(�쿬�� ������ �� �ִ� �ð�)
        float difficulty = 0;
        switch(ClickCount / 10) // �̹��� �ٲ�� �ð��� ����Ͽ� 0.5f�� ����
        {
            case 1: difficulty = 3f; break;
            case 2: difficulty = 2f; break;
            default: difficulty = 1f; break;
        }

        Animator accidyAnimator = accidy.GetComponent<Animator>();
        accidyAnimator.SetBool("isMove", true);

        // 3�ʿ��� 6�� ���� ������ �ð� ���� �쿬�� ������
        float moveTime = Random.Range(2.5f, 5.5f), currentTime = 0;
        while (!isGameOver)
        {
            currentTime += Time.deltaTime;

            if(currentTime > moveTime)
            {
                // �쿬�� �������� ���� (���̵��� ���� �쿬�� ������ �ִ� �ð��� �ٸ��� ����)
                accidyAnimator.SetBool("isMove", false);
                yield return new WaitForSeconds(difficulty);

                // �쿬�� ����� �ٲ�� �ִϸ��̼�
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.2f, true, 0, 0));
                yield return new WaitForSeconds(0.2f);
                accidy.sprite = accidyBackSprite;
                accidy.SetNativeSize();
                yield return new WaitForSeconds(0.2f);

                // �쿬�� �ڸ� ���ƺ� ���� (1�� �� ����)
                accidyBack = true;
                yield return new WaitForSeconds(2);
                accidyBack = false;

                // �ٽ� ���� ���� �ִϸ��̼�
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.2f, true, 0, 0));
                yield return new WaitForSeconds(0.2f);
                accidy.sprite = accidyFrontSprite;
                accidy.SetNativeSize();
                yield return new WaitForSeconds(0.2f);

                // ������ �ʱ�ȭ
                currentTime = 0;
                moveTime = Random.Range(2.5f, 5.5f);

                // �ٽ� �쿬�� �����̴� �ִϸ��̼� ����
                accidyAnimator.SetBool("isMove", true);
            }
            yield return null;
        }
    }
}
