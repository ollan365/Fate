using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room2ActionPointManager : ActionPointManager
{
    // ȸ���� �Ծ��� ���
    [SerializeField] private bool isEatenEnergySupplement;

    // ��Ż��2���� ȸ���� ���� ���¸� ������� extended�� �ٲ�
    [SerializeField] private GameObject backgroundImageDefault;
    [SerializeField] private GameObject backgroundImageExtended;

    private bool isChoosingBrokenBearChoice = false;

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

        GameManager.Instance.AddEventObject("EventRoom2HomeComing");
        GameManager.Instance.AddEventObject("EventRoom2Morning");
    }

    // create 5 hearts on screen on room start
    public override void CreateHearts()
    {
        int actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        int heartCount = presentHeartIndex + 1;

        // ȸ���� �Ծ actionPoint�� 2�� �� �þ
        if (isEatenEnergySupplement)
        {
            // ��Ʈ ���� actionPointsPerDay�� �� ���� ���̸� DecrementActionPoint���� ��Ʈ�� 0�� �Ǹ� 
            // �������� �ִ� ��Ʈ�� �̸� ������Ʈ �صα⿡ ��Ʈ�� 0�� �� ���� ��. ��Ʈ�� 0������ 2���� �������
            if (heartCount == actionPointsPerDay)
            {
                nowDayNum -= 1;
                GameManager.Instance.SetVariable("NowDayNum", nowDayNum);

                // presentHeartIndex�� 2-1�� ������Ʈ
                presentHeartIndex = 1;

                actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
            }
            else
            {
                actionPointsPerDay = 7;
                GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
                // actionPointPerDay�� ����Ǿ� �ٽ� actionPointsArray ����
                CreateActionPointsArray(actionPointsPerDay);

                presentHeartIndex += 2;
            }

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

        for (int i = 0; i < heartCount; i++)
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";

        // ��Ʈ ����� �ٲ�
        ChangeHeartBackgroundImageExtended((bool)GameManager.Instance.GetVariable("TeddyBearFixed"));

        if (isEatenEnergySupplement)
            isEatenEnergySupplement = false;

        //Debug.Log(heartParent.transform.childCount);
    }

    public override void DecrementActionPoint()
    {
        if ((bool)GameManager.Instance.GetVariable("TeddyBearFixed"))
        {
            actionPointsPerDay = 7;
            GameManager.Instance.SetVariable("ActionPointsPerDay", actionPointsPerDay);
            // actionPointPerDay�� ����Ǿ� �ٽ� actionPointsArray ����
            CreateActionPointsArray(actionPointsPerDay);
        }

        // �ð� ���񿡼� �������� Ŭ������ �� ����Ʈ ���� ���� ���� �ʰ� ��
        if (heartParent.transform.childCount < 1)
            return;

        // pop heart on screen
        GameObject heart = heartParent.transform.GetChild(presentHeartIndex).gameObject;

        // animate heart by triggering "break" animation
        heart.GetComponent<Animator>().SetTrigger("Break");

        // deactivate heart after animation
        StartCoroutine(DeactivateHeart(heart));

        presentHeartIndex--;

        int actionPoint;

        // ��Ʈ�� �� ��������
        if (presentHeartIndex == -1)
        {
            if (nowDayNum < maxDayNum)
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


    // �Ͱ� ��ũ��Ʈ ��� �κ�
    public override void RefillHeartsOrEndDay()
    {
        // turn off all ImageAndLockPanel objects and zoom out
        RoomManager.Instance.ExitToRoot();

        // if all action points are used, load "Follow 1" scene
        int actionPoint = (int)GameManager.Instance.GetVariable("ActionPoint");
        if (actionPoint == 0)
        {
            DialogueManager.Instance.StartDialogue("RoomEscape2S_016");
            SceneManager.Instance.LoadScene(Constants.SceneType.FOLLOW_2);

            return;
        }
        // �Ͱ� ��ũ��Ʈ ���
        EventManager.Instance.CallEvent("EventRoom2HomeComing");

        GameManager.Instance.SetVariable("RefillHeartsOrEndDay", false);
        // �Ͱ� ��ũ��Ʈ ���� ������ Next�� Event_NextMorningDay fade in/out ����Ʈ ����
    }

    // ����(��ħ) ��ũ��Ʈ ��� �κ�
    public override void nextMorningDay()
    {
        // �������� �ǰ�(fade in/out effect ����) ��ħ ��ũ��Ʈ ���
        const float totalTime = 3f;
        StartCoroutine(ScreenEffect.Instance.DayPass(totalTime));  // fade in/out effect

        // ��ħ ��ũ��Ʈ ���
        EventManager.Instance.CallEvent("EventRoom2Morning");

        // ���⼭ ��Ʈ ���� �� �������� ��¥ ������Ʈ
        StartCoroutine(RefillHearts(totalTime / 2));
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

        // ��Ʈ ����� �ٲ�
        ChangeHeartBackgroundImageExtended(true);
    }

    public void SetChoosingBrokenBearChoice(bool isChoosing)
    {
        isChoosingBrokenBearChoice = isChoosing;
    }

    public bool GetChoosingBrokenBearChoice()
    {
        return isChoosingBrokenBearChoice;
    }

    // ȸ���� ���� ���¶�� ������� �ٲ�
    private void ChangeHeartBackgroundImageExtended(bool isExtended)
    {
        int DefaultMaxActionPoint = 5;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Room2")
        {
            if (isExtended)
            {
                if (backgroundImageExtended.activeSelf) return;
                else
                {
                    if ((presentHeartIndex + 1) > DefaultMaxActionPoint)
                    {
                        backgroundImageDefault.SetActive(false);
                        backgroundImageExtended.SetActive(true);
                    }
                }
            }
            else
            {
                backgroundImageDefault.SetActive(true);
                backgroundImageExtended.SetActive(false);
            }
        }
    }

}