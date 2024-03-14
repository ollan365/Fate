using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject diary_p2;

    [SerializeField]
    private GameObject diary_p3;

    [SerializeField]
    private RoomMovManager roomMov;

    public static bool clearDiary = false;

    [SerializeField]
    private GameObject laptop_login;

    [SerializeField]
    private GameObject laptop_p1;

    public static bool clearLaptop = false;

    [SerializeField]
    private GameObject Clock_bg;

    [SerializeField]
    private GameObject key_bg;

    public static bool clearClock = false;

    // �繰����.....��ư���� �ع�����...�ش� �繰 ��ư ������
    // �ܼ� �� â ���� �� �ٸ� ��ư���� ������.. �װ� ���� �뵵...
    public GameObject diary;
    public GameObject laptop;
    public GameObject clock;

    //void Start() // ���⸦ ����θ� Diary_p2�� �� ã�Ƽ� �׳� �ּ� ó�� �߽��ϴ� �۵��� �� �ϴ� �� ���ƿ�!
    //{
    //    diary_p2 = GameObject.Find("Diary_p2");
    //    roomMov = GetComponent<RoomMovManager>();

    //    diary = GameObject.Find("Diary");
    //    laptop = GameObject.Find("Laptop");
    //    clock = GameObject.Find("Clock");
    //}

    //void Update()
    //{
        
    //}

    public void diaryBtn()
    {
        if (!clearDiary)
        {
            // �ٸ� ��ư�� �� ���̰� ��
            laptop.SetActive(false);
            clock.SetActive(false);

            roomMov.isResearch = true;
            diary_p2.SetActive(true);
        }
        else
        {
            Debug.Log("�̹� ���캻 �ܼ���.");
        }
    }

    public void laptopBtn()
    {
        if (!clearLaptop)
        {
            diary.SetActive(false);
            clock.SetActive(false);

            roomMov.isResearch = true;
            laptop_login.SetActive(true);
        }
        else
        {
            Debug.Log("�̹� ���캻 �ܼ���.");
        }
    }

    public void clockBtn()
    {
        if (!clearClock)
        {
            diary.SetActive(false);
            laptop.SetActive(false);

            roomMov.isResearch = true;
            Clock_bg.SetActive(true);
        }
        else
        {
            Debug.Log("�̹� ���캻 �ܼ���.");
        }
    }

    public void SearchExitBtn()
    {
        if (diary_p2.activeSelf)
        {
            roomMov.isResearch = false;
            diary_p2.SetActive(false);
        }
        else if (diary_p3.activeSelf)
        {
            roomMov.isResearch = false;
            diary_p3.SetActive(false);

            clearDiary = true;
        }

        if (laptop_p1.activeSelf)
        {
            roomMov.isResearch = false;
            laptop_login.SetActive(false);
            laptop_p1.SetActive(false);

            clearLaptop = true;
        }
        else if (laptop_login.activeSelf)
        {
            roomMov.isResearch = false;
            laptop_login.SetActive(false);
        }

        if (key_bg.activeSelf)
        {
            roomMov.isResearch = false;
            key_bg.SetActive(false);
            Clock_bg.SetActive(false);

            clearClock = true;
        }
        else if (Clock_bg.activeSelf)
        {
            roomMov.isResearch = false;
            Clock_bg.SetActive(false);
        }

        diary.SetActive(true);
        laptop.SetActive(true);
        clock.SetActive(true);
    }
}
