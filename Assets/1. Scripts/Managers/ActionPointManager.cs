using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//public enum RoomType
//{
//    Room1,
//    Room2
//}

abstract public class ActionPointManager : MonoBehaviour
{
    // ************************* temporary members for action points *************************
    public GameObject heartPrefab;
    public GameObject heartParent;
    public TextMeshProUGUI dayText;

    [SerializeField] protected int maxDayNum;   // ��Ż�⿡�� ������ �ִ� �ϼ�
    [SerializeField] protected int nowDayNum;
    [SerializeField] protected int actionPointsPerDay;

    // �ൿ�� ���ҷ� ���� ��Ʈ �ڸ�
    [SerializeField] protected int presentHeartIndex;

    // ������ actionPointsPerDay�� ���� �޶����� actionpoints�迭
    protected int[,] actionPointsArray;

    [SerializeField] protected Q_Vignette_Single WarningVignette;

    [SerializeField] protected float warningTime;

    // ************************* temporary methods for action points *************************
    // create actionPointsArray
    protected void CreateActionPointsArray(int actionPointsPerDay)
    {
        actionPointsArray = new int[maxDayNum, actionPointsPerDay];

        int actionPoint = 1;
        for (int day = maxDayNum - 1; day >= 0; day--)
        {
            for (int index = 0; index < actionPointsPerDay; index++)
            {
                actionPointsArray[day, index] = actionPoint;
                actionPoint++;
            }
        }
    }

    // create 5 hearts on screen on room start
    public abstract void CreateHearts();

    public abstract void DecrementActionPoint();

    // �Ͱ� ��ũ��Ʈ ��� �κ�
    public abstract void RefillHeartsOrEndDay();

    // ����(��ħ) ��ũ��Ʈ ��� �κ�
    public abstract void nextMorningDay();

    protected static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
    }

    protected IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();
    }

    // ħ�뿡�� �޽��ϸ� �ൿ�� ������ �������� �Ѿ
    // Day1�� ��Ʈ 4�� �����־ Day2�� �Ѿ�� actionPointsPerDay ��ŭ ä����
    public IEnumerator TakeRest()
    {
        float time = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(time));   // fade in/out effect

        // �޽� ��� ���. 
        StartCoroutine(DialogueManager.Instance.StartDialogue("RoomEscape_035", time));

        yield return new WaitForSeconds(time / 2);

        // �޽��ϸ� �׳� �Ϸ翡 �����ִ� �ൿ�� �� ���Ǳ⿡ ���� �ִ� ��Ʈ�� ����
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        int actionPoint;

        if (nowDayNum < maxDayNum) 
        {
            nowDayNum += 1;
            presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;

            GameManager.Instance.SetVariable("NowDayNum", nowDayNum);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

        }
        else
        {
            // ������ ���� 5�Ͽ� �޽����� ���
            // �ൿ���� 0�� �� ����
            actionPoint = 0;
        }

        GameManager.Instance.SetVariable("ActionPoint", actionPoint);

        SaveManager.Instance.SaveGameData();
    }

    protected IEnumerator Warning()
    {
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        // actionPoint�� 0���� Ŭ ���� �ڷ�ƾ ����
        if (actionPoint <= 0)
        {
            yield break; // actionPoint�� 0 ���ϸ� �ڷ�ƾ ����
        }

        float start = 0, end = 1;
        float fadeInTime = 0.5f;  // ��� ǥ�� ���̵� �� �ð�
        float fadeOutTime = 0.5f;  // ��� ���� ���̵� �ƿ� �ð�
        float current = 0, percent = 0;

        // ��� ����: WarningVignette.mainColor.a�� start���� end�� ���̵� ��
        while (percent < 1 && fadeInTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeInTime;

            // vignette�� ����(alpha)�� 0���� 1�� ���� ����(Lerp)
            WarningVignette.mainColor.a = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        // warningTime ���� ��� ���� ����
        yield return new WaitForSeconds(warningTime);

        // ��� ����: WarningVignette.mainColor.a�� �ٽ� 0���� ���̵� �ƿ�
        current = 0;
        percent = 0;

        while (percent < 1 && fadeOutTime != 0)
        {
            current += Time.deltaTime * 2;  // ���̵� �ƿ� �ӵ��� �� ������ ����
            percent = current / fadeOutTime;

            // WarningVignette ������ 1���� 0���� ���� ����(Lerp)
            WarningVignette.mainColor.a = Mathf.Lerp(end, start, percent);

            yield return null;
        }

        // �ڷ�ƾ ���� �� WarningVignette.mainColor.a�� 0���� �����Ͽ� ��� ������ �����
        WarningVignette.mainColor.a = 0;
    }


}
