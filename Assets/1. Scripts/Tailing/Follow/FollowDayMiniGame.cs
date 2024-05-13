using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FollowDayMiniGame : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject followCanvas;
    [SerializeField] private GameObject followUICanvas;
    [SerializeField] private GameObject miniGameCanvas;

    [Header("UI")]
    [SerializeField] private Slider[] gaugeSliders;
    [SerializeField] private Animator[] heartAnimator;
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject[] buttonImages;

    [Header("Character")]
    [SerializeField] private Image accidy;
    [SerializeField] private Image[] fatePositions;
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

            if (clickCount % 10 != 0) return;

            for (int i = 0; i < 3; i++) clear[i] = true;

            switch (ClickCount / 10) // �̹��� �ٲ�� �ð��� ����Ͽ� 0.5f�� ����
            {
                case 1:
                    difficulty = 3f;
                    gaugeSliders[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttons[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.MEMO)].gameObject.SetActive(true);

                    // Ŭ���� �ؾ��ϴ� ��
                    clear[ToInt(Place.MEMO)] = false;
                    break;
                case 2:
                    difficulty = 2f;
                    gaugeSliders[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttons[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.FIRST)].gameObject.SetActive(true);

                    // Ŭ���� �ؾ��ϴ� ��
                    clear[ToInt(Place.MEMO)] = false;
                    clear[ToInt(Place.FIRST)] = false;
                    break;
                default:
                    difficulty = 1f;
                    gaugeSliders[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttons[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.THIRD)].gameObject.SetActive(true);

                    // Ŭ���� �ؾ��ϴ� ��
                    clear[ToInt(Place.MEMO)] = false;
                    clear[ToInt(Place.FIRST)] = false;
                    clear[ToInt(Place.THIRD)] = false;
                    break;
            }

             StartCoroutine(StartMiniGame());
        }
    }

    // ���º���
    public int heartCount = 3; // ��� ����
    private bool isGameOver; // ���� ���� �Ǿ��°�
    private bool[] clear; // �������� �� ä���°�
    private bool accidyBack; // �쿬�� �ڸ� ���� �ִ°�
    private bool canClick;
    private float difficulty = 0; // ���̵�
    public enum Place { WALL, FIRST, MEMO, THIRD }
    [SerializeField] private Place currentPlace; // �ν����Ϳ��� Ȯ��
    private int ToInt(Place place)
    {
        switch (place)
        {
            case Place.FIRST: return 0;
            case Place.MEMO: return 1;
            case Place.THIRD: return 2;
            case Place.WALL: return 3;
        }
        return -1;
    }
    private void Start()
    {
        clear = new bool[3];

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
        foreach (Slider s in gaugeSliders) s.value = 0;
        currentPlace = Place.WALL;

        // �ʿ��� ��ġ �ʱ�ȭ
        foreach (Image i in fatePositions) i.color = new Color(1, 1, 1, 0);
        fatePositions[ToInt(Place.WALL)].color = new Color(1, 1, 1, 1);

        // �޸� ��ư ���ֱ�
        MemoManager.Instance.HideMemoButton(true);

        // ���̵� �ƿ��� ���� �ϸ� ���� ĵ������ ���� �̴� ���� ĵ������ �Ҵ� + ����� �ٲ۴�
        followUICanvas.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW);
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
        yield return new WaitForSeconds(0.4f); // ���̵� �� �ƿ� ��

        // �쿬�� ������ ����
        StartCoroutine(AccidyLogic());

        // ��ư�� Ȱ��ȭ
        canClick = true;

        // ���� �ڿ��� �ѵ��� ����
        bool onHit = false;

        // ���� ���� or ���� Ŭ������� �ݺ�
        while (!isGameOver)
        {
            // ���� Ŭ����
            if (clear[ToInt(Place.MEMO)] && clear[ToInt(Place.FIRST)] && clear[ToInt(Place.THIRD)]) break;

            // �ʿ��� �������� �쿬�� �ڸ� ���ƺ� ���°� ��ø�Ǹ� ��Ʈ �ϳ� ����
            if (currentPlace != Place.WALL && accidyBack && !onHit)
            {
                onHit = true;
                heartCount--;

                if (heartCount <= 0) isGameOver = true;
                else // ü���� 1 ��� �ٽ� �������� ���ư���
                {
                    accidyBack = false; // �Ѵ� �¾����� �� �̻� ü���� ������ �ʵ��� �Ѵ�
                    StartCoroutine(FateMove(Place.WALL));

                    // ��Ʈ�� ������ �ִϸ��̼� ���
                    heartAnimator[heartCount].SetTrigger("Break");

                    // ȭ���� �Ӿ����� �ִϸ��̼�
                    ScreenEffect.Instance.coverPanel.color = Color.red;
                    StartCoroutine(ScreenEffect.Instance.OnFade(null, 0.5f, 0, 0.2f, false, 0, 0));
                    yield return new WaitForSeconds(0.5f);

                    heartAnimator[heartCount].gameObject.SetActive(false);
                    ScreenEffect.Instance.coverPanel.color = Color.black;
                }
                onHit = false;
            }
            yield return null;
        }

        if (isGameOver) // ���� ����
        {

        }
        else // ���� Ŭ����
        {
            isGameOver = true; // �ٸ� �ڷ�ƾ���� ���ߵ��� ����

            MemoManager.Instance.HideMemoButton(false);

            StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
            SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
            yield return new WaitForSeconds(0.2f);
            miniGameCanvas.SetActive(false);
            followCanvas.SetActive(true);
            SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW);
            yield return new WaitForSeconds(0.4f); // ���̵� �� �ƿ� ��
            followUICanvas.SetActive(true);
        }
    }

    public void ButtonOnClick(int placeNum)
    {
        if (!canClick) return;

        Place place = Place.WALL;

        switch (placeNum)
        {
            case 0: place = Place.FIRST; break;
            case 1: place = Place.MEMO; break;
            case 2: place = Place.THIRD; break;
        }

        if (currentPlace == place && currentPlace != Place.WALL) // ���� ��ҿ� Ŭ���� ���� ���� ��
        {
            gaugeSliders[ToInt(place)].value += 0.015f;

            if(gaugeSliders[ToInt(place)].value == 1) clear[ToInt(place)] = true;
        }
        else if(currentPlace != place) // �ʿ��� �����ִ� ���� or ������ ����
        {
            StartCoroutine(FateMove(place));
        }
    }
    private IEnumerator FateMove(Place place)
    {
        canClick = false; // �ִϸ��̼� ���� ������ ��ư ��Ȱ��ȭ

        // �ʿ��� ��ġ�� �ٲ�� �ִϸ��̼�
        StartCoroutine(ScreenEffect.Instance.OnFade(fatePositions[ToInt(currentPlace)], 1, 0, 0.2f, false, 0, 0));
        yield return new WaitForSeconds(0.2f);

        // �ʿ��� ���¸� �̵��� ��ġ�� ����
        currentPlace = place;

        StartCoroutine(ScreenEffect.Instance.OnFade(fatePositions[ToInt(place)], 0, 1, 0.2f, false, 0, 0));
        yield return new WaitForSeconds(0.2f);

        canClick = true;
    }
    private IEnumerator AccidyLogic()
    {
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
