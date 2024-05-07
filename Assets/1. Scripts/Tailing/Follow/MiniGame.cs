using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
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
    [SerializeField] private GameObject fateBack;
    [SerializeField] private GameObject[] fateSit;
    [SerializeField] private Sprite accidyGirlFront, accidyGirlBack, accidyBoyFront, accidyBoyBack;
    private Sprite accidyFrontSprite, accidyBackSprite;

    // 물체 클릭 횟수
    private float clickCount;
    public float ClickCount
    {
        get => clickCount;
        set
        {
            clickCount = value;

            switch (ClickCount / 10) // 이미지 바뀌는 시간을 고려하여 0.5f씩 줄임
            {
                case 1:
                    difficulty = 3f;
                    gaugeSliders[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttons[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.MEMO)].gameObject.SetActive(true);
                    break;
                case 2:
                    difficulty = 2f;
                    gaugeSliders[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttons[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.FIRST)].gameObject.SetActive(true);
                    break;
                default:
                    difficulty = 1f;
                    gaugeSliders[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttons[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    buttonImages[ToInt(Place.THIRD)].gameObject.SetActive(true);
                    break;
            }

            if (clickCount % 10 == 0) StartCoroutine(StartMiniGame());
        }
    }

    // 상태변수
    private int heartCount = 3; // 목숨 개수
    private bool isGameOver; // 게임 오버 되었는가
    private bool accidyBack; // 우연이 뒤를 보고 있는가
    private bool fateMove; // 필연이 이동을 했는가
    private bool canClick;
    private float difficulty = 0; // 난이도
    public enum Place { WALL, FIRST, MEMO, THIRD }
    private Place currentPlace;
    private int ToInt(Place place)
    {
        switch (place)
        {
            case Place.WALL: return -1;
            case Place.FIRST: return 0;
            case Place.MEMO: return 1;
            case Place.THIRD: return 2;
        }
        return -1;
    }
    private void Start()
    {
        // 우연의 성별에 따라 다른 이미지
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
        // 변수들 초기화
        isGameOver = false;
        accidyBack = false;
        fateMove = false;
        foreach (Slider s in gaugeSliders) s.value = 0;
        currentPlace = Place.WALL;

        // 메모 버튼 없애기
        MemoManager.Instance.HideMemoButton(true);

        // 페이드 아웃과 인을 하며 미행 캔버스를 끄고 미니 게임 캔버스를 켠다 + 브금을 바꾼다
        followUICanvas.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW);
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
        yield return new WaitForSeconds(0.4f); // 페이드 인 아웃 끝

        // 우연의 움직임 시작
        StartCoroutine(AccidyLogic());

        // 버튼들 활성화
        canClick = true;

        // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 하트 하나 감소
        while (!isGameOver)
        {
            if(fateMove && accidyBack)
            {
                heartCount--;

                if (heartCount <= 0) isGameOver = true;
                else // 체력을 1 깎고 다시 벽쪽으로 돌아간다
                {
                    fateMove = false;
                    StartCoroutine(FateMove(Place.WALL));

                    // 하트가 터지는 애니메이션 재생
                    heartAnimator[heartCount].SetTrigger("Break");

                    // 화면이 붉어지는 애니메이션
                    ScreenEffect.Instance.coverPanel.color = Color.red;
                    StartCoroutine(ScreenEffect.Instance.OnFade(null, 0.5f, 0, 0.2f, false, 0, 0));
                    yield return new WaitForSeconds(0.5f);

                    heartAnimator[heartCount].gameObject.SetActive(false);
                    ScreenEffect.Instance.coverPanel.color = Color.black;
                }
            }

            yield return null;
        }

        // 게임 오버 (현재는 무조건 미행으로 돌아감)
        MemoManager.Instance.HideMemoButton(false);

        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
        yield return new WaitForSeconds(0.2f);
        miniGameCanvas.SetActive(false);
        followCanvas.SetActive(true);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW);
        yield return new WaitForSeconds(0.4f); // 페이드 인 아웃 끝
        followUICanvas.SetActive(true);
    }

    public void MemoButtonOnClick(Place place)
    {
        if (!canClick) return;

        if (!fateMove) // 필연이 숨어있는 상태였다면
        {
            StartCoroutine(FateMove(place));
        }
        else
        {
            gaugeSliders[ToInt(place)].value += 0.015f;

            if(gaugeSliders[ToInt(place)].value == 1) isGameOver = true; // 이거 수정
        }
    }
    public void WallButtonOnClick()
    {
        StartCoroutine(FateMove(Place.WALL));
    }
    private IEnumerator FateMove(Place place)
    {
        canClick = false; // 애니메이션 끝날 때까지 버튼 비활성화

        if (place != Place.WALL)
        {
            // 필연의 위치가 바뀌는 애니메이션
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 1, 0, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit[ToInt(place)].GetComponent<Image>(), 0, 1, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);

            // 필연의 상태를 이동한 상태로 변경
            fateMove = true;
            currentPlace = place;
        }
        else
        {
            // 필연의 상태를 이동하지 않은 상태로 변경
            fateMove = false;

            // 필연의 위치가 바뀌는 애니메이션
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit[ToInt(currentPlace)].GetComponent<Image>(), 1, 0, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 0, 1, 0.2f, false, 0, 0));
            yield return new WaitForSeconds(0.2f);

            // 필연의 위치 변경
            currentPlace = place;
        }
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
