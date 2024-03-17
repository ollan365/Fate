using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMovManager : MonoBehaviour
{
    public GameObject roomP1;
    public GameObject roomP2;
    public GameObject roomP3;

    // ���� �̺�Ʈ ���̸� �������� ���ϰ� �ϴ� ��������
    public bool isResearch = false;

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
}
