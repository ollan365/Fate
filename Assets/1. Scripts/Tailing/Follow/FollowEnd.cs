using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnd : MonoBehaviour
{
    [SerializeField] private FollowGameManager followGameManager;
    [SerializeField] private GameObject frontCanvas;
    private Animator Accidy { get => FollowManager.Instance.Accidy; }
    private Animator Fate { get => FollowManager.Instance.Fate; }
    private Camera mainCam;
    private float zoomTime = 1.5f;
    private enum Position { Fate, Accidy, ZoomOut }
    private void Start()
    {
        mainCam = Camera.main;
    }
    public IEnumerator EndFollow()
    {
        // 필연으로 줌인
        StartCoroutine(ZoomIn(Position.Fate));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // 스크립트 "Follow1Fianal" 출력 + 느낌표
        FollowManager.Instance.blockingPanel.SetActive(true);
        frontCanvas.SetActive(false);
        DialogueManager.Instance.StartDialogue("Follow1Final_001");

        // 스크립트가 끝날 때까지 대기
        while (FollowManager.Instance.blockingPanel.activeSelf)
            yield return null;
        frontCanvas.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // 우연으로 줌인
        StartCoroutine(ZoomIn(Position.Accidy));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // 우연 대사 출력
        FollowManager.Instance.blockingPanel.SetActive(true);
        frontCanvas.SetActive(false);
        DialogueManager.Instance.StartDialogue("Follow1Final_002");

        // 스크립트가 끝날 때까지 대기
        while (FollowManager.Instance.blockingPanel.activeSelf)
            yield return null;
        frontCanvas.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // 우연이 뒤돌아봄
        Accidy.SetBool("Back", true);
        SoundPlayer.Instance.UISoundPlay(Constants.Sound_TurnAround);
        yield return new WaitForSeconds(0.5f);

        // 다시 필연 쪽으로 줌인
        StartCoroutine(ZoomIn(Position.Fate));
        yield return new WaitForSeconds(zoomTime + 0.5f);

        // 뒷걸음질 3번 후 뒤돌아서 1.5배속 달리기
        SoundPlayer.Instance.UISoundPlay(Constants.Sound_FollowEnd);
        Fate.speed = 0.6f;
        Fate.SetBool("Walking", true);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(MoveFate());
            yield return new WaitForSeconds(1f);
        }

        // 카메라 원래대로
        StartCoroutine(ZoomIn(Position.ZoomOut));

        // 페이드 아웃
        Fate.SetBool("Walking", false);
        Fate.SetTrigger("Turn");
        followGameManager.ChangeAnimStatusToStop(false);
        yield return new WaitForSeconds(1f);

        // 미행 끝
        SceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
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
                targetPosition = new(Fate.transform.position.x, -2, -10);
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
            // 보간하여 카메라 위치와 크기를 변경
            mainCam.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / zoomTime);
            mainCam.orthographicSize = Mathf.Lerp(originSize, targetSize, elapsedTime / zoomTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 변경이 완료된 후 최종 목표값으로 설정
        mainCam.transform.position = targetPosition;
        mainCam.orthographicSize = targetSize;
    }

    // === 미행이 끝났을 때 === //
    public IEnumerator MoveFate()
    {
        Accidy.gameObject.SetActive(false); // 꺼두지 않으면 카메라 원상 복귀 때 화면에 잡힘

        Vector3 originPosition = Fate.transform.position;
        Vector3 targetPosition = originPosition + new Vector3(-0.3f, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < 1.2f)
        {
            Fate.transform.position = Vector3.Lerp(originPosition, targetPosition, elapsedTime / 1.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Fate.transform.position = targetPosition;
    }
}
