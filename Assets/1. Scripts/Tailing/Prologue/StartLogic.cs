using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class StartLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text, choiceTextA, choiceTextB;
    [SerializeField] private GameObject[] choiceBTNs;
    [SerializeField] private Button[] prologueBTN;

    public void GoScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
