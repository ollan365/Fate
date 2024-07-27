using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = Unity.Mathematics.Random;

public class ActionPointManager : MonoBehaviour
{
    // ************************* temporary members for action points *************************
    public GameObject heartPrefab;
    public GameObject heartParent;
    public TextMeshProUGUI dayText;

    public int maxDayNum;   // ��Ż�⿡�� ������ �ִ� �ϼ�
    public int nowDayNum;
    public int actionPointsPerDay;

    // �ൿ�� ���ҷ� ���� ��Ʈ �ڸ�
    public int presentHeartIndex;
    // ȸ���� �Ծ��� ���
    private bool isEatenEnergySupplement;

    // ������ actionPointsPerDay�� ���� �޶����� actionpoints�迭
    private int[,] actionPointsArray;

    private void Awake()
    {
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");
        isEatenEnergySupplement = (bool)GameManager.Instance.GetVariable("IsEatenEnergySupplement");

        CreateActionPointsArray(actionPointsPerDay);

        // ó�� ��Ż���� actionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);
    }

    // ************************* temporary methods for action points *************************
    // create actionPointsArray
    private void CreateActionPointsArray(int actionPointsPerDay)
    {
        actionPointsArray = new int[5, actionPointsPerDay];

        int actionPoint = 1;
        for (int row = maxDayNum - 1; row >= 0; row--)
        {
            for (int col = 0; col < actionPointsPerDay; col++)
            {
                actionPointsArray[row, col] = actionPoint;
                actionPoint++;
            }
        }
    }

    // create 5 hearts on screen on room start
    public void CreateHearts()
    {
        int actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        //int heartCount = actionPoint % actionPointsPerDay;
        int heartCount = presentHeartIndex + 1;

        // ȸ���� �Ծ actionPoint�� 2�� �� �þ
        if (isEatenEnergySupplement)
        {
            actionPointsPerDay = 7;
            GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
            // actionPointPerDay�� ����Ǿ� �ٽ� actionPointsArray ����
            CreateActionPointsArray(actionPointsPerDay);

            presentHeartIndex += 2;

            heartCount = presentHeartIndex + 1;

            // actionPoint�� 2�� �� �þ ���·� ����
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];

            GameManager.Instance.SetVariable("ActionPoint", actionPoint);
            GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);
        }
        
        // ��Ʈ�� 0�� �Ǹ�
        if (heartCount == 0)
        {
            if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
            {
                // ȸ���� ���� ���¸� �Ϸ翡 ��Ʈ �ִ� 5������ ���� 7���� �þ
                if (actionPointsPerDay == 5)
                {
                    actionPointsPerDay = 7;
                    GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
                }
            }
            heartCount = actionPointsPerDay;
        }

        //Debug.Log("heartCount : " + heartCount);
        //Debug.Log("actionPoint : " + actionPoint);
        //Debug.Log("�� �Ŵ����� actionPoint : " + GameManager.Instance.GetVariable("ActionPoint"));

        for (int i = 0; i < heartCount; i++)
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";

        if (isEatenEnergySupplement)
        {
            isEatenEnergySupplement = false;
        }
    }

    public void DecrementActionPoint()
    {
        if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
        {
            actionPointsPerDay = 7;
            GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
            // actionPointPerDay�� ����Ǿ� �ٽ� actionPointsArray ����
            CreateActionPointsArray(actionPointsPerDay);
        }

        if (presentHeartIndex > -1)
        {
            // pop heart on screen
            GameObject heart = heartParent.transform.GetChild(presentHeartIndex).gameObject;

            // animate heart by triggering "break" animation
            heart.GetComponent<Animator>().SetTrigger("Break");

            // deactivate heart after animation
            StartCoroutine(DeactivateHeart(heart));

        }
        presentHeartIndex--;

        int actionPoint;

        if (presentHeartIndex == -1)
        {
            if(nowDayNum < 5)
            {
                // ���� ��¥�� �������� ������Ʈ
                nowDayNum += 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndex�� �� �� row�� ������Ʈ
                presentHeartIndex = (int)GameManager.Instance.GetVariable("ActionPointsPerDay") - 1;
                GameManager.Instance.SetVariable("PresentHeartIndex", presentHeartIndex);

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                // ������ ��
                // �ൿ���� 0�� �� ����
                actionPoint = 0;
            }
        }
        else
        {
            // actionPoint ������Ʈ�ϰ� GameManager�� ActionPoint�� ����
            actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        }
        
        GameManager.Instance.SetVariable("ActionPoint", actionPoint);

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            if (!isDialogueActive) RefillHeartsOrEndDay();
            else GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }

        SaveManager.Instance.SaveGameData();
    }

    public void RefillHeartsOrEndDay()
    {
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            SceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
            return;
        }
        // �Ͱ� ��ũ��Ʈ ���
        var randomHomeComing = new Random((uint)System.DateTime.Now.Ticks);  // choose a random dialogue ID

        var HomeComingDialogueID = "";
        switch (nowDayNum)
        {
            case 2:
                //������� 1���� �Ͱ�
                string[] random1HomeComingDialogueIDs = { "RoomEscapeS_001", "RoomEscapeS_003" };
                HomeComingDialogueID = random1HomeComingDialogueIDs[randomHomeComing.NextInt(0, 2)];
                break;

            case 3:
                //Ư����� 2���� �Ͱ�
                HomeComingDialogueID = "RoomEscapeS_005";
                break;

            case 4:
                //Ư����� 3���� �Ͱ�
                HomeComingDialogueID = "RoomEscapeS_008";
                break;

            case 5:
                //������� 4���� �Ͱ�
                string[] random2HomeComingDialogueIDs = { "RoomEscapeS_012", "RoomEscapeS_013" };
                HomeComingDialogueID = random2HomeComingDialogueIDs[randomHomeComing.NextInt(0, 2)];
                break;

            default:
                break;
        }
        DialogueManager.Instance.StartDialogue(HomeComingDialogueID);
        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // �Ͱ� ��ũ��Ʈ ���� ������ Next�� Event_NextMorningDay fade in/out ����Ʈ ����
    }

    public void nextMorningDay()
    {
        // �������� �ǰ�(fade in/out effect ����) ��ħ ��ũ��Ʈ ���
        const float totalTime = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        var MorningDialogueID = "";

        switch (nowDayNum)
        {
            case 2:
                //Ư�� ���
                MorningDialogueID = "RoomEscapeS_004";
                break;

            case 3: case 4:
                //Ư�� ���
                MorningDialogueID = "RoomEscapeS_015";
                break;

            case 5:
                //Ư�� ���
                MorningDialogueID = "RoomEscapeS_014";
                break;
        }
        StartCoroutine(DialogueManager.Instance.StartDialogue(MorningDialogueID, totalTime));

        // ���⼭ ��Ʈ ���� �� �������� ��¥ ������Ʈ
        StartCoroutine(RefillHearts(totalTime / 2));
    }


    private static IEnumerator DeactivateHeart(Object heart)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
    }

    private IEnumerator RefillHearts(float totalTime)
    {
        yield return new WaitForSeconds(totalTime);
        CreateHearts();
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();
    }

    // ������ �� ��� ������ ������ ��ũ��Ʈ ������ �ٷ� ��Ʈ 2�� ȸ����.
    public void EatEnergySupplement()
    {
        // �ϴ� ���� ���̴� ��Ʈ �����
        foreach (Transform child in heartParent.transform)
        {
            Destroy(child.gameObject);
        }

        // ��Ʈ +2�� �߰��ϰ�
        // ��Ʈ �ٽ� ����� �ؼ� ��Ʈ�� 2�� �� ä���� ��ó�� ���̰� ��.

        isEatenEnergySupplement = true;

        CreateHearts();
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

        if (nowDayNum < 5) 
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
}
