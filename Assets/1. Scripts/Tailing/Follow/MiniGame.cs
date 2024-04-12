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

    [Header("Character")]
    [SerializeField] private Image accidy;
    [SerializeField] private GameObject fateBack, fateSit;
    [SerializeField] private Sprite accidyFrontSprite, accidyBackSprite;

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
    private bool isGameOver; // 게임 오버 되었는가
    private bool accidyBack; // 우연이 뒤를 보고 있는가
    private bool fateMove; // 필연이 이동을 했는가

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
        float moveTime = Random.Range(5, 8), currentTime = 0;
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
