using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnd : MonoBehaviour
{
    private Camera mainCam;
    private float zoomTime = 1.5f;
    private enum Position { Fate, Accidy }
    private void Start()
    {
        mainCam = Camera.main;
    }
    public IEnumerator EndFollow()
    {
        // �ʿ����� ����
        StartCoroutine(ZoomIn(Position.Fate));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // ��ũ��Ʈ "Follow1Fianal" ��� + ����ǥ
        FollowManager.Instance.blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("Follow1Fianal_001");

        // ��ũ��Ʈ�� ���� ������ ���
        while (FollowManager.Instance.blockingPanel.activeSelf)
            yield return null;
        yield return new WaitForSeconds(0.5f);

        // �쿬���� ����
        StartCoroutine(ZoomIn(Position.Accidy));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // �쿬 ��� ���
        FollowManager.Instance.blockingPanel.SetActive(true);
        DialogueManager.Instance.StartDialogue("Follow1Fianal_002");

        // ��ũ��Ʈ�� ���� ������ ���
        while (FollowManager.Instance.blockingPanel.activeSelf)
            yield return null;
        yield return new WaitForSeconds(0.5f);

        // �쿬�� �ڵ��ƺ�
        FollowManager.Instance.followAnim.ChangeAnimStatusOnEnd(0);
        yield return new WaitForSeconds(0.5f);

        // �ٽ� �ʿ� ������ ����
        StartCoroutine(ZoomIn(Position.Fate));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // �ް����� 3�� �� �ڵ��Ƽ� 1.5��� �޸���
        FollowManager.Instance.followAnim.ChangeAnimStatusOnEnd(1);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(FollowManager.Instance.followAnim.MoveFate());
            SoundPlayer.Instance.UISoundPlay(Constants.Sound_FootStep_Fate);
            yield return new WaitForSeconds(1f);
        }

        // ȭ�� ��� ���� ���� ���鼭 ���̵� �ƿ�
        FollowManager.Instance.followAnim.ChangeAnimStatusOnEnd(2);
        yield return new WaitForSeconds(1f);
        StartCoroutine(ScreenEffect.Instance.OnFade(null, 0, 1, 2, false, 0, 0));

        // ���� ����
        SaveManager.Instance.SaveGameData();

        // ���� ��
        FollowManager.Instance.FollowEnd();
    }

    private IEnumerator ZoomIn(Position type)
    {
        Vector3 originPosition = mainCam.transform.position;
        float originSize = mainCam.orthographicSize;

        Vector3 targetPosition = new(0, 0, -10);
        float targetSize = 5;

        switch (type)
        {
            case Position.Fate:
                targetPosition = new(0, -2, -10);
                targetSize = 3;
                break;

            case Position.Accidy:
                targetPosition = new(8, -2, -10);
                targetSize = 3;
                break;

            default: break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < zoomTime)
        {
            // �����Ͽ� ī�޶� ��ġ�� ũ�⸦ ����
            mainCam.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / zoomTime);
            mainCam.orthographicSize = Mathf.Lerp(originSize, targetSize, elapsedTime / zoomTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ������ �Ϸ�� �� ���� ��ǥ������ ����
        mainCam.transform.position = targetPosition;
        mainCam.orthographicSize = targetSize;
    }
}
