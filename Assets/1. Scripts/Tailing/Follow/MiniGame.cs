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
    [SerializeField] private GameObject[] heartImages;
    [SerializeField] private Button memoButton;

    [Header("Character")]
    [SerializeField] private Image accidy;
    [SerializeField] private GameObject fateBack, fateSit;
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

            if (clickCount % 10 == 0) StartCoroutine(StartMiniGame());
        }
    }

    // 상태변수
    private int heartCount = 3; // 목숨 개수
    private bool isGameOver; // 게임 오버 되었는가
    private bool accidyBack; // 우연이 뒤를 보고 있는가
    private bool fateMove; // 필연이 이동을 했는가

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
    }
    private IEnumerator StartMiniGame()
    {
        // 변수들 초기화
        isGameOver = false;
        accidyBack = false;
        fateMove = false;
        gaugeSlider.value = 0;

        // 페이드 아웃과 인을 하며 미행 캔버스를 끄고 미니 게임 캔버스를 켠다
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        yield return new WaitForSeconds(0.4f); // 페이드 인 아웃 끝

        // 우연의 움직임 시작
        StartCoroutine(AccidyLogic());

        // 떨어진 메모장의 버튼 활성화
        memoButton.gameObject.SetActive(true);

        // 필연이 움직였고 우연이 뒤를 돌아본 상태가 중첩되면 하트 하나 감소
        while (!isGameOver)
        {
            if(fateMove && accidyBack)
            {
                heartCount--;

                if (heartCount < 0) isGameOver = true;
                else // 체력을 1 깎고 다시 벽쪽으로 돌아간다
                {
                    fateMove = false;
                    heartImages[heartCount].SetActive(false);
                    StartCoroutine(FateMove(false));
                }
            }
            else if (!fateMove) // 필연이 움직이지 않으면 천천히 게이지 감소
            {
                gaugeSlider.value -= 0.0001f;
            }

            yield return null;
        }

        // 게임 오버
        // 추후 구현 필요!!!
    }

    public void MemoButtonOnClick()
    {
        if (!fateMove) // 필연이 숨어있는 상태였다면
        {
            StartCoroutine(FateMove(true));
        }
        else gaugeSlider.value += 0.05f;
    }
    public void WallButtonOnClick()
    {
        StartCoroutine(FateMove(false));
    }
    private IEnumerator FateMove(bool goToMemo)
    {
        memoButton.gameObject.SetActive(false); // 애니메이션 끝날 때까지 버튼 비활성화

        if (goToMemo)
        {
            // 필연의 위치가 바뀌는 애니메이션
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 1, 0, 0.25f, false, 0, 0));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit.GetComponent<Image>(), 0, 1, 0.25f, false, 0, 0));
            yield return new WaitForSeconds(0.25f);

            // 필연의 상태를 이동한 상태로 변경
            fateMove = true;
        }
        else
        {
            // 필연의 상태를 이동하지 않은 상태로 변경
            fateMove = false;

            // 필연의 위치가 바뀌는 애니메이션
            StartCoroutine(ScreenEffect.Instance.OnFade(fateSit.GetComponent<Image>(), 1, 0, 0.25f, false, 0, 0));
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(ScreenEffect.Instance.OnFade(fateBack.GetComponent<Image>(), 0, 1, 0.25f, false, 0, 0));
            yield return new WaitForSeconds(0.25f);
        }
        memoButton.gameObject.SetActive(true);
    }
    private IEnumerator AccidyLogic()
    {
        // 난이도(우연이 가만히 서 있는 시간)
        float difficulty = 0;
        switch(ClickCount / 10) // 이미지 바뀌는 시간을 고려하여 0.5f씩 줄임
        {
            case 1: difficulty = 2.5f; break;
            case 2: difficulty = 1.5f; break;
            default: difficulty = 0.5f; break;
        }

        Animator accidyAnimator = accidy.GetComponent<Animator>();
        accidyAnimator.SetBool("isMove", true);

        // 3초에서 6초 사이 랜덤한 시간 동안 우연이 움직임
        float moveTime = Random.Range(3, 6), currentTime = 0;
        while (!isGameOver)
        {
            currentTime += Time.deltaTime;

            if(currentTime > moveTime)
            {
                // 우연의 움직임을 멈춤 (난이도에 따라 우연이 가만히 있는 시간을 다르게 적용)
                accidyAnimator.SetBool("isMove", false);
                yield return new WaitForSeconds(difficulty);

                // 우연의 모습이 바뀌는 애니메이션
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.25f, true, 0, 0));
                yield return new WaitForSeconds(0.25f);
                accidy.sprite = accidyBackSprite;
                yield return new WaitForSeconds(0.25f);

                // 우연이 뒤를 돌아본 상태 (1초 간 유지)
                accidyBack = true;
                yield return new WaitForSeconds(2);
                accidyBack = false;

                // 다시 앞을 보는 애니메이션
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.25f, true, 0, 0));
                yield return new WaitForSeconds(0.25f);
                accidy.sprite = accidyFrontSprite;
                yield return new WaitForSeconds(0.25f);

                // 변수들 초기화
                currentTime = 0;
                moveTime = Random.Range(3, 6);

                // 다시 우연이 움직이는 애니메이션 시작
                accidyAnimator.SetBool("isMove", true);
            }
            yield return null;
        }
    }
}
