using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLogic : MonoBehaviour
{

    // 튜토리얼1, 2 관련 (시점 이동키 누르기)
    public void Tutorial_MoveLeft()
    {
        switch ((int)GameManager.Instance.GetVariable("Tutorial_Now"))
        {
            case 1:
                // Tutorial1_Event_002_B
                if (EventManager.Instance)
                {
                    EventManager.Instance.CallEvent("Tutorial1_Event_002");
                    EventManager.Instance.CallEvent("Tutorial1_Event_003_A");
                    EventManager.Instance.CallEvent("Tutorial1_Event_004_B");
                }
                break;

            case 2:
                //Tutorial2_Event_001
                if (EventManager.Instance)
                {
                    EventManager.Instance.CallEvent("Tutorial2_Event_001");
                }
                break;
        }
    }
    public void Tutorial_MoveRight()
    {
        switch ((int)GameManager.Instance.GetVariable("Tutorial_Now"))
        {
            case 1:
                //Tutorial1_Event_002_A
                if (EventManager.Instance)
                {
                    EventManager.Instance.CallEvent("Tutorial1_Event_002");
                    EventManager.Instance.CallEvent("Tutorial1_Event_003_B");
                    EventManager.Instance.CallEvent("Tutorial1_Event_004_A");
                }
                break;

            case 2:
                //Tutorial2_Event_001
                if (EventManager.Instance)
                {
                    EventManager.Instance.CallEvent("Tutorial2_Event_001");
                }
                break;
        }
    }

    // 튜토리얼3 관련
    // (자퇴서 확대 UI 나오고 조사 스크립트 다 보고 나가기 버튼 누르면 실행됨)
    public void Tutorial_ExitButton()
    {
        switch ((int)GameManager.Instance.GetVariable("Tutorial_Now"))
        {
            case 3:
                //Tutorial3_Event_001
                if (EventManager.Instance)
                {
                    EventManager.Instance.CallEvent("Tutorial3_Event_001");
                }
                break;
        }
    }
}
