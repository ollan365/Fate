using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    public GameObject roomP1;
    public GameObject roomP2;
    public GameObject roomP3;

    // ���� �̺�Ʈ ���̸� �������� ���ϰ� �ϴ� ��������
    public bool isResearch = false;

    public List<GameObject> ScreenObjects = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        roomP1.SetActive(true);
        roomP2.SetActive(false);
        roomP3.SetActive(false);
    }

    // ���� Ŭ�� �̺�Ʈ �߻��ؼ� ���� ���̸� A�� D�� ������ �� ����� ��
    void Update()
    {
        if (!isResearch)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // P1 -> P2
                // P2 -> P3
                // P3 -> P1
                if (roomP1.activeSelf)
                {
                    roomP1.SetActive(false);
                    roomP2.SetActive(true);
                }
                else if (roomP2.activeSelf)
                {
                    roomP2.SetActive(false);
                    roomP3.SetActive(true);
                }
                else
                {
                    roomP3.SetActive(false);
                    roomP1.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // P1 -> P3
                // P2 -> P1
                // P3 -> P2
                if (roomP1.activeSelf)
                {
                    roomP1.SetActive(false);
                    roomP3.SetActive(true);
                }
                else if (roomP2.activeSelf)
                {
                    roomP2.SetActive(false);
                    roomP1.SetActive(true);
                }
                else
                {
                    roomP3.SetActive(false);
                    roomP2.SetActive(true);
                }
            }
        }

    }

    public void SearchExitBtn()
    {
        DeactivateObjects();
        isResearch = false;
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in ScreenObjects)
        {
            obj.SetActive(false);
        }
        // ����Ʈ Ŭ����� ������ �ϸ� ���� ����� �� ������� ������ �־ Ŭ����� ���ϰ� ��...
        //ScreenObjects.Clear();
    }

    public void AddScreenObjects(GameObject obj)
    {
        if (!ScreenObjects.Contains(obj)) ScreenObjects.Add(obj);
    }
}
