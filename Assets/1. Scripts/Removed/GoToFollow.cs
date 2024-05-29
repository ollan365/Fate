using UnityEngine;
public class GoToFollow : MonoBehaviour
{
    public void GoFollowScene()
    {
        SceneManager.Instance.LoadScene(Constants.SceneType.FOLLOW_1);
    }
}
