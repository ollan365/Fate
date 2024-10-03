using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1ActionPointManager : ActionPointManager
{
    private void Awake()
    {
        maxDayNum = (int)GameManager.Instance.GetVariable("MaxDayNum");
        nowDayNum = (int)GameManager.Instance.GetVariable("NowDayNum");
        actionPointsPerDay = (int)GameManager.Instance.GetVariable("ActionPointsPerDay");
        presentHeartIndex = (int)GameManager.Instance.GetVariable("PresentHeartIndex");

        CreateActionPointsArray(actionPointsPerDay);

        // ó�� ��Ż���� actionPoint
        GameManager.Instance.SetVariable("ActionPoint", actionPointsArray[0, presentHeartIndex]);


        GameManager.Instance.AddEventObject("EventRoom1HomeComing");
        GameManager.Instance.AddEventObject("EventRoom1Morning");

    }

    // create 5 hearts on screen on room start
    public override void CreateHearts()
    {
        int actionPoint = actionPointsArray[nowDayNum - 1, presentHeartIndex];
        // 25 action points -> 5 hearts, 24 action points -> 4 hearts, so on...
        int heartCount = presentHeartIndex + 1;

        // ��Ʈ�� 0�� �Ǹ�
        if (heartCount == 0)
        {
            heartCount = actionPointsPerDay;
        }

        for (int i = 0; i < heartCount; i++)
        {
            // create heart on screen by creating instances of heart prefab under heart parent
            Instantiate(heartPrefab, heartParent.transform);
        }

        // change Day text on screen
        dayText.text = $"Day {nowDayNum}";

        //Debug.Log(heartParent.transform.childCount);
    }

    public override void DecrementActionPoint()
    {
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

        StartCoroutine(Warning());

        if (actionPoint % actionPointsPerDay == 0)
        {
            bool isDialogueActive = DialogueManager.Instance.isDialogueActive;
            if (!isDialogueActive) RefillHeartsOrEndDay();
            else GameManager.Instance.SetVariable("RefillHeartsOrEndDay", true);
        }

        SaveManager.Instance.SaveGameData();
    }

    public override void RefillHeartsOrEndDay()
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
        EventManager.Instance.CallEvent("EventRoom1HomeComing");

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
        EventManager.Instance.CallEvent("EventRoom1Morning");

        // ���⼭ ��Ʈ ���� �� �������� ��¥ ������Ʈ
        StartCoroutine(RefillHearts(totalTime / 2));
    }

}
