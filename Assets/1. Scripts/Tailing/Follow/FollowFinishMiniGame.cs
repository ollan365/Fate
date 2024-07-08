using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FollowFinishMiniGame : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject followCanvas;
    [SerializeField] private GameObject followUICanvas;
    [SerializeField] private GameObject finishGameCanvas;
    [SerializeField] private GameObject finishGameObjects;
    [SerializeField] private GameObject finishGameEndCanvas;

    [Header("UI")]
    [SerializeField] private Animator[] heartAnimator;
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private Sprite[] accidySprite;
    [SerializeField] private GameObject tutorialCanvas;

    [Header("Object")]
    [SerializeField] private GameObject fate;
    [SerializeField] private GameObject fateEnd;
    [SerializeField] private GameObject accidyEnd;
    [SerializeField] private GameObject[] obstructionPrefabs;
    [SerializeField] private Transform[] fatePositions;
    [SerializeField] private Transform[] obstructionPositions;

    [Header("Variable")]
    [SerializeField] private float gamePlayTime;
    [SerializeField] private float moveTime;
    [SerializeField] private float invincibleTime;
    public bool isGameOver = false;
    private int heartCount;

    [Header("Background")]
    [SerializeField] private Transform[] backgrounds;
    [SerializeField] private float scrollAmount;
    [SerializeField] private float backgroundMoveSpeed;

    public IEnumerator FinishGameStart(int heartCount)
    {
        // 변수
        this.heartCount = heartCount;
        for (int i = 0; i < heartCount; i++) heartAnimator[i].gameObject.SetActive(true);

        // 우연의 성별에 따라 다른 이미지
        if ((int)GameManager.Instance.GetVariable("AccidyGender") == 0)  accidyEnd.GetComponent<Image>().sprite = accidySprite[0];
        else accidyEnd.GetComponent<Image>().sprite = accidySprite[1];
        accidyEnd.GetComponent<Image>().SetNativeSize();

        // 페이드 아웃과 인을 하며 미행 캔버스를 끄고 엔드 게임 캔버스를 켠다
        followUICanvas.SetActive(false);
        followCanvas.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 1, 0, 0.2f, false, 0, 0));
        yield return new WaitForSeconds(0.2f);
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_FINISHGAME, true);
        finishGameCanvas.SetActive(true);
        finishGameObjects.SetActive(true);

        // 튜토리얼 캔버스를 끌 때까지 기다린다
        while (tutorialCanvas.activeSelf) yield return new WaitForFixedUpdate();

        StartCoroutine(BackgroundMove());

        float currentTime = 0;

        while (!isGameOver && currentTime < gamePlayTime)
        {
            int obstructionCount = Random.Range(1, 3); // 장애물은 1개 또는 2개
            int spawnPosition = Random.Range(0, 3);

            for (int i = 0; i < obstructionCount; i++)
            {
                spawnPosition = spawnPosition + i < 3 ? spawnPosition + i : 0;
                GameObject obstruction = Instantiate(obstructionPrefabs[Random.Range(0, 3)], obstructionPositions[spawnPosition].position, Quaternion.identity);

                obstruction.GetComponent<Obstruction>().followFinishMiniGame = this;
            }

            float randomTimn = Random.Range(2f, 3f); // 랜덤 간격으로 장애물 생성
            yield return new WaitForSeconds(randomTimn);
            currentTime += randomTimn;
        }

        isGameOver = true;

        // 메모의 개수가 부족할 때
        if (!MemoManager.Instance.UnlockNextScene())
        {
            SceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
            yield break;
        }

        //메모의 개수가 충분할 때
        // 미니 게임 끝 (페이드 인아웃)
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        finishGameObjects.SetActive(false);
        finishGameEndCanvas.SetActive(true);
        yield return new WaitForSeconds(0.4f);

        // 필연이 앞으로 걸어나옴
        while (true)
        {
            fateEnd.transform.position += Vector3.up * Time.deltaTime * 5;
            if (fateEnd.transform.position.y >= 2.3f)
            {
                fateEnd.transform.position = new(fateEnd.transform.position.x, 2.3f, fateEnd.transform.position.z);
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        // 우연이 앞으로 걸어나옴
        while (true)
        {
            accidyEnd.transform.position += Vector3.up * Time.deltaTime;
            if (accidyEnd.transform.position.y >= 0)
            {
                accidyEnd.transform.position = new(accidyEnd.transform.position.x, 0, accidyEnd.transform.position.z);
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        DialogueManager.Instance.dialogueType = Constants.DialogueType.ROOM;
        blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("Follow1Final_003"); // 우연의 대사 출력
    }
    public IEnumerator BackgroundMove() // 배경 움직이기 (미완성)
    {
        // fate.GetComponentInChildren<ParticleSystem>().Play();
        while (!isGameOver)
        {
            foreach (Transform t in backgrounds)
            {
                t.position += Vector3.down * backgroundMoveSpeed * Time.deltaTime;

                if (t.position.y <= -scrollAmount)
                {
                    t.position += Vector3.up * scrollAmount * 2;
                }
            }
            yield return null;
        }
        // fate.GetComponentInChildren<ParticleSystem>().Stop();
    }
    public IEnumerator OnHit()
    {
        heartCount--;

        // 화면이 붉어지는 애니메이션
        ScreenEffect.Instance.coverPanel.color = Color.red;
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0.5f, 0, 0.2f, false, 0, 0));

        if (heartCount <= 0) isGameOver = true; // 게임 오버
        else StartCoroutine(PlayerInvincible()); // 아직 생명이 남아있으면 무적 시간

        // 하트가 터지는 애니메이션 재생
        heartAnimator[heartCount].SetTrigger("Break");
        yield return new WaitForSeconds(0.5f);
        heartAnimator[heartCount].gameObject.SetActive(false);

        ScreenEffect.Instance.coverPanel.color = Color.black;
    }
    private IEnumerator PlayerInvincible()
    {
        fate.tag = "Untagged";
        fate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

        float current = 0;
        Color startColor = new Color(1, 1, 1, 1), endColor = new Color(1, 1, 1, 0);

        while (current < invincibleTime)
        {
            float t = 0;
            while (current < invincibleTime && t < 0.5f)
            {
                current += Time.deltaTime;
                t += Time.deltaTime;
                fate.GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            t = 0;

            while (current < invincibleTime && t < 0.5f)
            {
                current += Time.deltaTime;
                t += Time.deltaTime;
                fate.GetComponent<SpriteRenderer>().color = Color.Lerp(endColor, startColor, t);
                yield return null;
            }
        }

        fate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        fate.tag = "Player";
    }

    public void OnClickMoveButton(int index)
    {
        StartCoroutine(Move(index));
    }
    private IEnumerator Move(int index)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = fate.transform.position;
        Vector3 targetPosition = fatePositions[index].position;

        while (elapsedTime < moveTime)
        {
            fate.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fate.transform.position = targetPosition;
    }
}
