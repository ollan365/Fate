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

    // ��ü Ŭ�� Ƚ��
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

    // ���º���
    private bool isGameOver; // ���� ���� �Ǿ��°�
    private bool accidyBack; // �쿬�� �ڸ� ���� �ִ°�
    private bool fateMove; // �ʿ��� �̵��� �ߴ°�

    private IEnumerator StartMiniGame()
    {
        // ������ �ʱ�ȭ
        isGameOver = false;
        accidyBack = false;
        fateMove = false;
        gaugeSlider.value = 0;

        // ���̵� �ƿ��� ���� �ϸ� ���� ĵ������ ���� �̴� ���� ĵ������ �Ҵ�
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 0.2f, true, 0.2f, 0));
        yield return new WaitForSeconds(0.2f);
        followCanvas.SetActive(false);
        miniGameCanvas.SetActive(true);
        yield return new WaitForSeconds(0.4f); // ���̵� �� �ƿ� ��

        // �쿬�� ������ ����
        StartCoroutine(AccidyLogic());

    }

    private IEnumerator AccidyLogic()
    {
        // ���̵�(�쿬�� ������ �� �ִ� �ð�)
        float difficulty = 0;
        switch(ClickCount / 10) // �̹��� �ٲ�� �ð��� ����Ͽ� 0.5f�� ����
        {
            case 1: difficulty = 2.5f; break;
            case 2: difficulty = 1.5f; break;
            default: difficulty = 0.5f; break;
        }

        Animator accidyAnimator = accidy.GetComponent<Animator>();
        accidyAnimator.SetBool("isMove", true);

        // 3�ʿ��� 6�� ���� ������ �ð� ���� �쿬�� ������
        float moveTime = Random.Range(5, 8), currentTime = 0;
        while (!isGameOver)
        {
            currentTime += Time.deltaTime;

            if(currentTime > moveTime)
            {
                // �쿬�� �������� ���� (���̵��� ���� �쿬�� ������ �ִ� �ð��� �ٸ��� ����)
                accidyAnimator.SetBool("isMove", false);
                yield return new WaitForSeconds(difficulty);

                // �쿬�� ����� �ٲ�� �ִϸ��̼�
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.25f, true, 0, 0));
                yield return new WaitForSeconds(0.25f);
                accidy.sprite = accidyBackSprite;
                yield return new WaitForSeconds(0.25f);

                // �쿬�� �ڸ� ���ƺ� ���� (1�� �� ����)
                accidyBack = true;
                yield return new WaitForSeconds(2);
                accidyBack = false;

                // �ٽ� ���� ���� �ִϸ��̼�
                StartCoroutine(ScreenEffect.Instance.OnFade(accidy, 1, 0, 0.25f, true, 0, 0));
                yield return new WaitForSeconds(0.25f);
                accidy.sprite = accidyFrontSprite;
                yield return new WaitForSeconds(0.25f);

                // ������ �ʱ�ȭ
                currentTime = 0;
                moveTime = Random.Range(3, 6);

                // �ٽ� �쿬�� �����̴� �ִϸ��̼� ����
                accidyAnimator.SetBool("isMove", true);
            }
            yield return null;
        }
    }
}
