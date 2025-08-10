using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnd : MonoBehaviour
{
    [SerializeField] private FollowGameManager followGameManager;
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] 
    private Animator Accidy { get => FollowManager.Instance.Accidy; }
    private Animator Fate { get => FollowManager.Instance.Fate; }

    public IEnumerator EndFollowLogic_0()
    {
        // 필연으로 줌인
        yield return new WaitForSeconds(FollowManager.Instance.Zoom(FollowManager.Position.Fate) + 0.5f);

        // 스크립트 "Follow1Fianal" 출력 + 느낌표
        EventManager.Instance.CallEvent("EventFollowEndStep_0");

    }
    public IEnumerator EndFollowLogic_1()
    {
        yield return new WaitForSeconds(0.5f);

        // 우연으로 줌인
        yield return new WaitForSeconds(FollowManager.Instance.Zoom(FollowManager.Position.Accidy) + 0.5f);

        // 우연 대사 출력
        EventManager.Instance.CallEvent("EventFollowEndStep_2");
    }
    public IEnumerator EndFollowLogic_3()
    {
        yield return new WaitForSeconds(0.5f);

        // 우연이 뒤돌아봄
        Accidy.SetBool("Back", true);
        SoundPlayer.Instance.UISoundPlay(Constants.Sound_TurnAround);
        yield return new WaitForSeconds(0.5f);

        // 다시 필연 쪽으로 줌인
        yield return new WaitForSeconds(FollowManager.Instance.Zoom(FollowManager.Position.Fate) + 0.5f);

        // 뒷걸음질 3번
        SoundPlayer.Instance.UISoundPlay(Constants.Sound_FollowEnd);
        Fate.speed = 0.6f;
        Fate.SetBool("Walking", true);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(MoveFate());
            yield return new WaitForSeconds(1f);
        }

        // 뒤돌아서 1.5배속 달리기
        Fate.SetBool("Walking", false);
        Fate.SetTrigger("Turn");

        // 미행 끝
        StartCoroutine(DelayLoadScene());

        while (true)
        {
            Fate.transform.Translate(Vector3.left * 2 * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(1);
        GameSceneManager.Instance.LoadScene(Constants.SceneType.ENDING);
    }

    // === 미행이 끝났을 때 === //
    public IEnumerator MoveFate()
    {
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
