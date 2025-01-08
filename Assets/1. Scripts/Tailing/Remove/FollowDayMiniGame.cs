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
    [SerializeField] private GameObject blockingPanel;

    [Header("Character")]
    [SerializeField] private Image accidy;
    [SerializeField] private Image[] fatePositions;
    [SerializeField] private Sprite accidyGirlFront, accidyGirlBack, accidyBoyFront, accidyBoyBack;
    private Sprite accidyFrontSprite, accidyBackSprite;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private Image tutorialAccidyImage;
    [SerializeField] private Sprite[] tutorialAccidySprites;

    [Header("Variables")]
    // 물체 클릭 횟수
    private float clickCount;
    public float ClickCount
    {
        get => clickCount;
        set
        {
            clickCount = value;

            if (clickCount % 5 != 0) return;

            for (int i = 0; i < 3; i++) clear[i] = true;

            switch (ClickCount / 5) // 이미지 바뀌는 시간을 고려하여 0.5f씩 줄임
            {
                case 1:
                    difficulty = 3f;
                    gaugeSliders[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttons[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.MEMO)].gameObject.SetActive(true);

                    tutorialObject.SetActive(true);

                    // 클리어 해야하는 것
                    clear[ToInt(Place.MEMO)] = false;
                    break;
                case 2:
                    difficulty = 2f;
                    gaugeSliders[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttons[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.FIRST)].gameObject.SetActive(true);

                    // 클리어 해야하는 것
                    clear[ToInt(Place.MEMO)] = false;
                    clear[ToInt(Place.FIRST)] = false;
                    break;
                default:
                    difficulty = 1f;
                    gaugeSliders[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttons[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.THIRD)].gameObject.SetActive(true);

                    // 클리어 해야하는 것
                    clear[ToInt(Place.MEMO)] = false;
                    clear[ToInt(Place.FIRST)] = false;
                    clear[ToInt(Place.THIRD)] = false;
                    break;
            }

             StartCoroutine(StartMiniGame());
        }
    }

    // 상태변수
    public int heartCount = 3; // 목숨 개수
    private bool isGameOver; // 게임 오버 되었는가
    private bool[] clear; // 게이지를 다 채웠는가
    private bool accidyBack; // 우연이 뒤를 보고 있는가
    private bool canClick;
    private float difficulty = 0; // 난이도
    public enum Place { WALL, FIRST, MEMO, THIRD }
    [SerializeField] private Place currentPlace; // 인스펙터에서 확인
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

        // 우연의 성별에 따라 다른 이미지
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0)
        {
            accidyFrontSprite = accidyGirlFront;
            accidyBackSprite = accidyGirlBack;
            tutorialAccidyImage.sprite = tutorialAccidySprites[0];
        }
        else
        {
            accidyFrontSprite = accidyBoyFront;
            accidyBackSprite = accidyBoyBack;
            tutorialAccidyImage.sprite = tutorialAccidySprites[1];
        }
        accidy.sprite = accidyFrontSprite;
        accidy.SetNativeSize();
        tutorialAccidyImage.SetNativeSize();
    }
    private IEnumerator StartMiniGame()
    {
        // 변수들 초기화
        isGameOver = false;
        accidyBack = false;
        foreach (Slider s in gaugeSliders) s.value = 0;
        currentPlace = Place.WALL;

        // 필연이, 우연이 위치와 이미지 초기화
        foreach (Image i in fatePositions) i.color = new Color(1, 1, 1, 0);
        fatePositions[ToInt(Place.WALL)].color = new Color(1, 1, 1, 1);
        accidy.sprite = accidyFrontSprite;
        accidy.SetNativeSize();

        // 메모 버튼 없애기
        MemoManager.Instance.SetMemoButtons(false);

        // 페이드 아웃과 인을 하며 미행 캔버스를 끄고 미니 게임 캔버스를 켠다 + 브금을 바꾼다
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_STOP);
        followUICanvas.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        MemoManager.Instance.SetMemoButtons(false);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
        yield return new WaitForSeconds(0.4f); // 페이드 인 아웃 끝

        while (tutorialObject.activeSelf) yield return new WaitForFixedUpdate(); // 튜토리얼 캔버스가 켜져 있다면 대기

        // 우연의 움직임 시작
        StartCoroutine(AccidyLogic());

        // 버튼들 활성화
        canClick = true;

        // 맞은 뒤에는 한동안 무적
        bool onHit = false;

        // 게임 오버 or 게임 클리어까지 반복
        while (!isGameOver)
        {
            // 게임 클리어
            if (clear[ToInt(Place.MEMO)] && clear[ToInt(Place.FIRST)] && clear[ToInt(Place.THIRD)]) break;

            // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 하트 하나 감소
            if (currentPlace != Place.WALL && accidyBack && !onHit)
            {
                onHit = true;
                heartCount--;

                if (heartCount <= 0)
                {
                    isGameOver = true;
                }
                else // 체력을 1 깎고 다시 벽쪽으로 돌아간다
                {
                    accidyBack = false; // 한대 맞았으면 더 이상 체력이 깎이지 않도록 한다
                    StartCoroutine(FateMove(Place.WALL));
                }

                // 하트가 터지는 애니메이션 재생
                heartAnimator[heartCount].SetTrigger("Break");

                // 화면이 붉어지는 애니메이션
                ScreenEffect.Instance.coverPanel.color = Color.red;
                StartCoroutine(ScreenEffect.Instance.OnFade(null, 0.5f, 0, 0.2f, false, 0, 0));
                yield return new WaitForSeconds(0.5f);

                heartAnimator[heartCount].gameObject.SetActive(false);
                ScreenEffect.Instance.coverPanel.color = Color.black;
                
                onHit = false;
            }
            yield return null;
        }

        // 게임 오버이고 메모의 개수 충분할 때
        // if (isGameOver && MemoManager.Instance.UnlockNextScene())
        if (isGameOver)
        {
            DialogueManager.Instance.dialogueType = Constants.DialogueType.ROOM_ACCIDY;
            blockingPanel.SetActive(true);
            DialogueManager.Instance.StartDialogue("Follow1Final_003"); // 우연의 대사 출력
            yield break;
        }
        // 게임 오버이고 메모의 개수가 불충분 or 게임 클리어
        else
        {
            bool followEnd = false;
            if (isGameOver) followEnd = true;

            isGameOver = true; // 다른 코루틴들이 멈추도록 설정

            MemoManager.Instance.SetMemoButtons(true);

            SoundPlayer.Instance.ChangeBGM(Constants.BGM_STOP);
            StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
            yield return new WaitForSeconds(0.2f);
            miniGameCanvas.SetActive(false);
            followCanvas.SetActive(true);
            SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW1);
            yield return new WaitForSeconds(0.4f); // 페이드 인 아웃 끝
            followUICanvas.SetActive(true);

            // if (followEnd) FollowManager.Instance.FollowEndLogicStart(true);
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

        if (currentPlace == place && currentPlace != Place.WALL) // 현재 장소와 클릭한 곳이 같을 ??
        {
            gaugeSliders[ToInt(place)].value += 0.045f / difficulty;

            if(gaugeSliders[ToInt(place)].value == 1) clear[ToInt(place)] = true;
        }
        else if(currentPlace != place) // 필연이 숨어있는 상태 or 숨으러 갈때
        {
            StartCoroutine(FateMove(place));
        }
    }
    private IEnumerator FateMove(Place place)
    {
        canClick = false; // 애니메이션 끝날 때까지 버튼 비활성화

        // 필연의 위치가 바뀌는 애니메이션
        StartCoroutine(ScreenEffect.Instance.OnFade(fatePositions[ToInt(currentPlace)], 1, 0, 0.2f, false, 0, 0));
        yield return new WaitForSeconds(0.2f);

        // 필연의 상태를 이동한 위치로 변경
        currentPlace = place;

        StartCoroutine(ScreenEffect.Instance.OnFade(fatePositions[ToInt(place)], 0, 1, 0.2f, false, 0, 0));
        yield return new WaitForSeconds(0.2f);

        canClick = true;
    }
    private IEnumerator AccidyLogic()
    {
        Animator accidyAnimator = accidy.GetComponent<Animator>();
        accidyAnimator.SetBool("isMove", true);

        // 3초에서 6초 사이 랜덤한 시간 동안 우연이 움직임
        float moveTime = Random.Range(2.5f, 5.5f), currentTime = 0;
        while (!isGameOver)
        {
            currentTime += Time.deltaTime;

            if(currentTime > moveTime)
            {
                // 우연의 움직임을 멈춤 (난이도에 따라 우연이 가만히 있는 시간을 다르게 적용)
                accidyAnimator.SetBool("isMove", false);
                yield return new WaitForSeconds(difficulty);

                // 우연의 모습이 바뀌는 애니메이션
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.2f, true, 0, 0));
                yield return new WaitForSeconds(0.2f);
                accidy.sprite = accidyBackSprite;
                accidy.SetNativeSize();
                yield return new WaitForSeconds(0.2f);

                // 우연이 뒤를 돌아본 상태 (1초 간 유지)
                accidyBack = true;
                yield return new WaitForSeconds(2);
                accidyBack = false;

                if (isGameOver) break;

                // 다시 앞을 보는 애니메이션
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.2f, true, 0, 0));
                yield return new WaitForSeconds(0.2f);
                accidy.sprite = accidyFrontSprite;
                accidy.SetNativeSize();
                yield return new WaitForSeconds(0.2f);

                // 변수들 초기화
                currentTime = 0;
                moveTime = Random.Range(2.5f, 5.5f);

                // 다시 우연이 움직이는 애니메이션 시작
                accidyAnimator.SetBool("isMove", true);
            }
            yield return null;
        }
    }
}