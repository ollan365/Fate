using System.Collections;
using UnityEngine;

public class FollowFinishMiniGame : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject followCanvas;
    [SerializeField] private GameObject followUICanvas;
    [SerializeField] private GameObject finishGameCanvas;
    [SerializeField] private GameObject finishGameObjects;

    [Header("UI")]
    [SerializeField] private Animator[] heartAnimator;

    [Header("Object")]
    [SerializeField] private GameObject fate;
    [SerializeField] private GameObject[] obstructionPrefabs;
    [SerializeField] private Transform[] fatePositions;
    [SerializeField] private Transform[] obstructionPositions;

    [Header("Variable")]
    [SerializeField] private float gamePlayTime;
    [SerializeField] private float moveTime;
    private bool isGameOver = false;
    private int heartCount;
    private int currentPosition = 1;

    [Header("Background")]
    [SerializeField] private Transform[] backgrounds;
    [SerializeField] private float scrollAmount;
    [SerializeField] private float backgroundMoveSpeed;

    public IEnumerator FinishGameStart(int heartCount)
    {
        // ����
        this.heartCount = heartCount;
        for (int i = 0; i < heartCount; i++) heartAnimator[i].gameObject.SetActive(true);

        // ���̵� �ƿ��� ���� �ϸ� ���� ĵ������ ���� ���� ���� ĵ������ �Ҵ� + ����� �ٲ۴�
        followUICanvas.SetActive(false);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        SoundPlayer.Instance.ChangeBGM(Constants.BGM_FOLLOW);
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        finishGameCanvas.SetActive(true);
        // SoundPlayer.Instance.ChangeBGM(Constants.BGM_MINIGAME);
        yield return new WaitForSeconds(0.4f);
        finishGameObjects.SetActive(true); // ���̵� �� �ƿ� ��

        float currentTime = 0;

        while (!isGameOver && currentTime < gamePlayTime)
        {
            int obstructionCount = Random.Range(1, 3); // ��ֹ��� 1�� �Ǵ� 2��
            int spawnPosition = Random.Range(0, 3);

            for(int i = 0; i < obstructionCount; i++)
            {
                spawnPosition = spawnPosition + i < 3 ? spawnPosition + i : 0;
                GameObject obstruction = Instantiate(obstructionPrefabs[Random.Range(0,3)], obstructionPositions[spawnPosition].position, Quaternion.identity);
                
                obstruction.GetComponent<Obstruction>().followFinishMiniGame = this;
                obstruction.GetComponent<Obstruction>().speed = backgroundMoveSpeed;
            }

            float randomTimn = Random.Range(3, 5); // 3���� 5�� �������� ���� ��ֹ� ����
            yield return new WaitForSeconds(randomTimn);
            currentTime += randomTimn;
        }

        FollowManager.Instance.FollowEnd();
    }
    public IEnumerator Background() // ��� �����̱� (�̿ϼ�)
    {
        while (!isGameOver)
        {
            foreach (Transform t in backgrounds)
            {
                t.position += Vector3.down * backgroundMoveSpeed * Time.deltaTime;

                if (t.position.y <= -scrollAmount)
                {
                    // t.position = targets.position - Vector3.down * scrollAmount;
                }
            }
            yield return null;
        }
    }
    public IEnumerator OnHit()
    {
        heartCount--;

        // ��Ʈ�� ������ �ִϸ��̼� ���
        heartAnimator[heartCount].SetTrigger("Break");
        yield return new WaitForSeconds(0.5f);
        heartAnimator[heartCount].gameObject.SetActive(false);

        if (heartCount <= 0) isGameOver = true; // ���� ����
    }
    public void OnClickMoveButton(int index)
    {
        if (Mathf.Abs(currentPosition - index) == 1)
        {
            StartCoroutine(Move(index));
        }
    }
    private IEnumerator Move(int index)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = fate.transform.position;
        Vector3 targetPosition = fatePositions[index].position;

        while (elapsedTime < moveTime)
        {
            fate.transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentPosition = index;
        fate.transform.position = targetPosition;
    }
}
